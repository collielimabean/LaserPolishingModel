import units

class Parameter:
    """ 
        Represents a value with a unit, e.g. 5 meters / second.
        Currently unused. Eventually, we should integrate this class 
        in to provide robust unit checking.
    """

    def __init__(self, name, value, unit):
        self.name = name
        self.value = units.unit(unit)(value)
