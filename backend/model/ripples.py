import matplotlib.pyplot as plt
import numpy as np
from scipy import optimize

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
        return np.piecewise(
            grid, 
            [grid < r_peak_sq, grid >= r_peak_sq], 
            [lambda s: self.ifs_inner * np.sqrt(s) + self.z_valley, lambda s: self.ifs_outer * np.sqrt(s) + outer_region_offset]
        )


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
    spot = sp.create_spot()