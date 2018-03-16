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
        xy_bounds = np.linspace(-self.r_melt, self.r_melt, num_pts)
        x, y = np.meshgrid(xy_bounds, xy_bounds)
        #circular_region = (ifs_inner * ((x.^2 + y.^2).^0.5) + z_valley) .* (x.^2 + y.^2 < r_peak^2);
        #outer_region = (ifs_outer * ((x.^2 + y.^2).^0.5) + outer_region_offset) .* (x.^2 + y.^2 >= r_peak^2);
        #return circular_region + outer_region

def ripples():
    pass