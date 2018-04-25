from model import *
import os
import numpy as np

SURFACE_FILE_PATH = '../data/1-15-15_MilledH_1_20x.asc'

zygo = ZygoAsciiFile(os.path.abspath(SURFACE_FILE_PATH))
material = Material('Steel', stc=-0.49e-3, mu=5.5e-3,k=28.5, rho_c=6.05e6, T_m=1727, T_b=3134, absp=0.38, rho=7783)
laser = Laser(beam_radius=15e-6, pulse_duration=3e-6, avg_power=3, duty_cycle=0.25, pulse_frequency=20e3)
config = ForwardModelConfig(melt_time_assumption='standard', surface_absorption_method='standard',
                show_code_settings=False, show_material_properties=False, show_general_figures=True,
                show_debugging_figures=True, show_console_output=False)


outputCache = OutputCache()
run_forward_model(zygo, material, laser, outputCache, config)
outputCache.dump_to_console()
