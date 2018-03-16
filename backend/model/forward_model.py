from .forward_model_config import ForwardModelConfig
from .zygo import ZygoAsciiFile
import matplotlib.pyplot as plt
import numpy as np


def run_capillary(zygo, material, laser, config=ForwardModelConfig()):
    if config.show_code_settings:
        print(str(config))

def run_thermocapillary(zygo, material, laser, config=ForwardModelConfig()):
    if config.show_code_settings:
        print(str(config))
