import matplotlib.pyplot as plt
import numpy as np

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

def ripples():
    pass