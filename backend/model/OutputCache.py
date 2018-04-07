from abc import ABC, abstractmethod
import plotly.plotly as py
import plotly.graph_objs as go


class OutputFigure(ABC):
    def __init__(self, figtype):
        self._figtype = figtype

    def serialize(self):
        data = self.serialize_data()
        return { 'type' : self._figtype, **data }

    @abstractmethod
    def serialize_data(self):
        pass

class Scatter(OutputFigure):
    def __init__(self, x, y, **kwargs):
        super(figtype='scatter').__init__()

class Surface(OutputFigure):
    def __init__(self, x, y, **kwargs):
        super(figtype='surface').__init__()

class OutputCache:
    def __init__(self, web=False):
        self.output_dict = {}
        self.figures = []

    def add_output(self, name, value, unit):
        pass

    def add_figure(self, name, figure):
        self.figures.append(figure.serialize())