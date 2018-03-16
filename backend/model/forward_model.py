from .forward_model_config import ForwardModelConfig
from .zygo import ZygoAsciiFile
import matplotlib.pyplot as plt
import numpy as np


def run_capillary(zygo, config = ForwardModelConfig()):
    if config.show_code_settings:
        print(str(config))

def run_thermocapillary(zygo, config = ForwardModelConfig()):
    if config.show_code_settings:
        print(str(config))