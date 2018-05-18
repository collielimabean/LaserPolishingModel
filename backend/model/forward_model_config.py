
class ForwardModelConfig:
    """
    Represents level of graph output that the forward model should produce.
    """

    def __init__(self, melt_time_assumption='standard', surface_absorption_method='standard',
                show_code_settings=False, show_material_properties=False, show_general_figures=False,
                show_debugging_figures=False, show_console_output=False):
        """ 
            Constructs a new ForwardModelConfig object.

            melt_time_assumption - [string] Standard, Justin, Nicolas, Brodan  
            surface_absorption_method - [string] Standard, SurfaceRay   %%% TODO - ADD SURFACE ABSOPRTION OPTIONS
        """

        self.melt_time_assumption = melt_time_assumption
        self.surface_absorption_method = surface_absorption_method
        self.show_code_settings = show_code_settings
        self.show_material_properties = show_material_properties
        self.show_general_figures = show_general_figures
        self.show_debugging_figures = show_debugging_figures
        self.show_console_output = show_console_output

    def __str__(self):
        return '\n'.join(["{}: {}".format(i[0], i[1]) for i in vars(self)])
