import matplotlib.pyplot as plt
import numpy as np
from scipy import optimize, interpolate
from zygo import ZygoAsciiFile

class SpotParameters:
    def __init__(self, ifs_inner, ifs_outer, r_melt, 
        r_peak, z_valley, z_peak, gridset):
        self.ifs_inner = ifs_inner
        self.ifs_outer = ifs_outer
        self.r_melt = r_melt
        self.r_peak = r_peak
        self.z_valley = z_valley
        self.z_peak = z_peak
        self.gridset = gridset

    def create_spot(self):
        num_pts = int((2 * self.r_melt) / self.gridset)
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
    def zero_vol_constraints(x):
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

    sol = optimize.root(zero_vol_constraints, [ifs_inner, ifs_outer, r_melt, r_peak, z_valley, z_peak], method='lm')
    return sol.x

def area_overlap(radius, step):
    return 2 * (radius ** 2) * np.arccos(step / (2 * radius)) - (step / 2) * np.sqrt(4 * (radius ** 2) - (step ** 2))

def ripples(XI, YI, afs_inner, d, spot, step):
    r_spot = spot / 2
    ifs_outer = 0
    ifs_inner = afs_inner
    r_melt = d / 2
    r_peak = r_melt - r_spot
    z_valley = 0
    z_peak = 0
    gridset = 0.2

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
    x_spots = 2 * np.ceil(1 / (1 - x_overlap_pct))
    y_spots = 2 * np.ceil(1 / (1 - y_overlap_pct))

    grid_x_limits = np.arange(-r_melt, gridset, (x_spots - 1) * x_step + r_melt)
    grid_y_limits = np.arange(-r_melt, gridset, (y_spots - 1) * y_step + r_melt)
    
    [overlay_x, overlay_y] = np.meshgrid(grid_x_limits, grid_y_limits)
    overlay_grid = np.zeros(np.size(overlay_x))

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

    uc_x_lim = [r_melt, (x_spots - 1) * x_step - r_melt]
    uc_y_lim = [r_melt, (y_spots - 1) * y_step - r_melt]
    uc_selector = (overlay_x > uc_x_lim[0]) and (overlay_x <= uc_x_lim[1]) and \
        (overlay_y > uc_y_lim[0]) and (overlay_y <= uc_y_lim[1])
    uc = overlay_grid(uc_selector)

    # pull the unit cell out and normalize it
    unit_cell_x = uc_x_lim / gridset
    unit_cell_y = uc_y_lim / gridset
    unit_cell = np.reshape(uc, unit_cell_x(2) - unit_cell_x(1), unit_cell_y(2) - unit_cell_y(1))
    unit_cell = unit_cell - np.mean(np.mean(unit_cell))
    
    # now take the unit cell and replicate to a size close to XI * YI
    # once again, 2x factor is for fudge factor to allow interp2 to do a
    # better job
    lattice = np.matlib.repmat(unit_cell, 2 * np.ceil(np.size(XI, 1) / np.size(unit_cell, 1)), 2 * np.ceil(np.size(YI, 0) / np.size(unit_cell, 0)))
    lattice_x = np.arange(0, gridset * (np.size(lattice, 1) - 1), gridset)
    lattice_y = np.arange(0, gridset * (np.size(lattice, 0) - 1), gridset)
    lattice_y = lattice_y.T
    
    # Convert to Experimental Mesh
    lattice_x = lattice_x / 1000
    lattice_y = lattice_y / 1000
    
    ip = interpolate.interp2d(lattice_x, lattice_y, lattice)
    return ip(XI, YI)
