import matplotlib.pyplot as plt
import numpy as np
from scipy import optimize, interpolate
from .zygo import ZygoAsciiFile

class SpotParameters:
    """ 
    The SpotParameters class generates a single laser spot from the following parameters.
    """
    def __init__(self, ifs_inner, ifs_outer, r_melt, 
        r_peak, z_valley, z_peak, gridset):
        """ Constructs a new SpotParameters object with the specified parameters."""
        self.ifs_inner = ifs_inner
        self.ifs_outer = ifs_outer
        self.r_melt = r_melt
        self.r_peak = r_peak
        self.z_valley = z_valley
        self.z_peak = z_peak
        self.gridset = gridset

    def create_spot(self):
        """Generates the conical spot shape, and returns a tuple of the resultant surface. """
        num_pts = int((2 * self.r_melt) / self.gridset) + 1
        r_peak_sq = np.power(self.r_peak, 2)
        outer_region_offset = self.z_peak - self.ifs_outer * self.r_peak

        xy_bounds = np.linspace(-self.r_melt, self.r_melt, num_pts)
        x, y = np.meshgrid(xy_bounds, xy_bounds)

        grid = np.power(x, 2) + np.power(y, 2)
        spot = np.piecewise(
            grid, 
            [grid < r_peak_sq, grid >= r_peak_sq], 
            [lambda s: self.ifs_inner * np.sqrt(s) + self.z_valley, lambda s: self.ifs_outer * np.sqrt(s) + outer_region_offset]
        )

        return [x, y, spot]

def compute_zero_volume_parameters(ifs_inner, ifs_outer, r_melt, r_peak, z_valley, z_peak):
    """
    Computes the necessary parameters to have a mass-conserved spot. 3 of 6 parametes supplied
    must be falsey (empty list, false, empty strings, etc), otherwise we cannot run the 
    computation as we would be overconstrained.
    """

    def zero_vol_constraints(x):
        """ Defines the 6 constraints necessary to have a mass-conserving spot."""
        # [ifs_inner, ifs_outer, r_melt, r_peak, z_valley, z_peak]
        constraints = [
            x[1] - ((x[0] * (x[3] ** 3)) / (x[3] ** 3 - x[2] ** 3)),
            x[4] + x[1] * (x[2] - x[3]),
            x[5] - x[4] + (x[0] * x[3])
        ]

        if ifs_inner:
            constraints.append(x[0] - ifs_inner)
    
        if ifs_outer:
            constraints.append(x[1] - ifs_outer)

        if r_melt:
            constraints.append(x[2] - r_melt)

        if r_peak:
            constraints.append(x[3] - r_peak)

        if z_valley:
            constraints.append(x[4] - z_valley)

        if z_peak:
            constraints.append(x[5] - z_peak)

        return constraints

    # solve for the constraints, and return the solution
    sol = optimize.root(zero_vol_constraints, [ifs_inner, ifs_outer, r_melt, r_peak, z_valley, z_peak], method='lm')
    return sol.x

def area_overlap(radius, step):
    """ Determines the ratio of overlap between circles with the same radii with the specified center offset."""
    return 2 * (radius ** 2) * np.arccos(step / (2 * radius)) - (step / 2) * np.sqrt(4 * (radius ** 2) - (step ** 2))

def ripples(XI, YI, afs_inner, d, spot, step):
    """ Generates the ripple surface by overlaying multiple spots."""
    r_spot = spot / 2
    ifs_outer = 0
    ifs_inner = afs_inner
    r_melt = d / 2
    r_peak = r_melt - r_spot
    z_valley = 0
    z_peak = 0
    gridset = 0.2

    # first, compute the mass-conserving spot.
    zero_vol_params = compute_zero_volume_parameters(ifs_inner, ifs_outer, r_melt, r_peak, z_valley, z_peak)
    sp = SpotParameters(*zero_vol_params, gridset)
    x, y, z = sp.create_spot()

    # let step either be a scalar (so x and y offsets are the same)
    # or a vector [x, y]
    if isinstance(spot, float) or isinstance(spot, int):
        x_step = step
        y_step = step
    else:
        x_step = step[1]
        y_step = step[2]

    roit = (x ** 2 + y ** 2) < (r_melt + gridset * 0.5) ** 2
    circular_region = z[roit]
    
    # plt.figure()
    # surf(x, y, z, 'LineStyle', 'none')
    
    ## Now overlay those spots over a grid
    # compute overlay
    # use the overlap area as a measure of how many spots we need to
    # generate the unit cell
    roit_proj_area = np.pi * (r_melt ** 2)
    x_area_overlap = area_overlap(r_melt, x_step)
    y_area_overlap = area_overlap(r_melt, y_step)

    x_overlap_pct = x_area_overlap / roit_proj_area
    y_overlap_pct = y_area_overlap / roit_proj_area
  
    # note the 2x factor is a "fudge" factor - unit cells seem better
    # this way
    x_spots = int(2 * np.ceil(1 / (1 - x_overlap_pct)))
    y_spots = int(2 * np.ceil(1 / (1 - y_overlap_pct)))

    # compute the grid x/y bounds
    lower_left_coord = (-r_melt, -r_melt)
    upper_right_coord = ((x_spots - 1) * x_step + r_melt, (y_spots - 1) * y_step + r_melt)
    grid_num_x = int(np.ceil(upper_right_coord[0] - lower_left_coord[0]) / gridset) + 1
    grid_num_y = int(np.ceil(upper_right_coord[1] - lower_left_coord[1]) / gridset) + 1
    grid_x_limits = np.linspace(lower_left_coord[0], upper_right_coord[0], grid_num_x)
    grid_y_limits = np.linspace(lower_left_coord[0], upper_right_coord[0], grid_num_y)
    
    [overlay_x, overlay_y] = np.meshgrid(grid_x_limits, grid_y_limits)
    overlay_grid = np.zeros(np.shape(overlay_x))

    # go in a snake pattern in the y direction
    # i.e. (1, 1) -> (1, n) -> (2, n) -> (2, 1) -> (3, 1) ...
    for i in range(x_spots):
        y_list = range(y_spots)
        if i % 2 == 0:
            y_list = y_list[::-1]

        for j in y_list:
            x_center = i * x_step
            y_center = j * y_step
            circular_region_selector = ((overlay_x - x_center) ** 2 + \
                (overlay_y - y_center) ** 2) < (r_melt + gridset * 0.5) ** 2
            overlay_grid[circular_region_selector] = circular_region

    # figure
    # surf(overlay_x, overlay_y, overlay_grid, 'LineStyle', 'none');

    # Unit cell to lattice
    # grab 'unit' from overlayed_grid
    uc_x_lim = np.array([r_melt, (x_spots - 1) * x_step - r_melt])
    uc_y_lim = np.array([r_melt, (y_spots - 1) * y_step - r_melt])
    uc_selector = np.logical_and(
        np.logical_and(overlay_x > uc_x_lim[0], overlay_x <= uc_x_lim[1]),
        np.logical_and(overlay_y > uc_y_lim[0], overlay_y <= uc_y_lim[1]))
    uc = overlay_grid[uc_selector]

    # pull the unit cell out and normalize it
    unit_cell_x = uc_x_lim / gridset
    unit_cell_y = uc_y_lim / gridset
    unit_cell = np.reshape(uc, [int(unit_cell_x[1] - unit_cell_x[0]), int(unit_cell_y[1] - unit_cell_y[0])])
    unit_cell = unit_cell - np.mean(unit_cell)
    
    # now take the unit cell and replicate to a size close to XI * YI
    # once again, 2x factor is for fudge factor to allow interp2 to do a
    # better job
    x_repl = int(2 * np.ceil(np.size(XI) / np.shape(unit_cell)[1]))
    y_repl = int(2 * np.ceil(np.size(YI) / np.shape(unit_cell)[0]))
    lattice = np.matlib.repmat(unit_cell, x_repl, y_repl)
    lattice_x = np.linspace(0, gridset * (np.size(lattice, 1) - 1), np.size(lattice, 1))
    lattice_y = np.linspace(0, gridset * (np.size(lattice, 0) - 1), np.size(lattice, 0))
    lattice_y = lattice_y.T
    
    # Convert to Experimental Mesh through interpolation
    lattice_x = lattice_x / 1000
    lattice_y = lattice_y / 1000
    
    ip = interpolate.interp2d(lattice_x, lattice_y, lattice)
    return ip(XI, YI)
