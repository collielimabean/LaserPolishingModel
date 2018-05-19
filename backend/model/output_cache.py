"""
output_cache.py defines the OutputCache class, which 
is responsible for holding general outputs (value or graph data)
in an agnostic manner. In other words, it will use Matplotlib PyPlot
if you are not using the web interface, but can create a JSON payload
if you are using the web interface.
"""

from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
from matplotlib import cm
import numpy as np
import json

class OutputCache:
    """
    Caches any output (e.g. a = 5) or graphs for future display or JSON bundling.
    As far as graphs go, only scatter and surface plots are supported. Furthermore, a 
    limited number of axis/title options are supported.
    """

    def __init__(self):
        """ Initializes an empty OutputCache object."""
        self.output_dict = {}
        self.output_figures = {}

    def add_output(self, name, value, unit):
        """ Adds an output with the specified name, value, and unit to the cache."""
        self.output_dict[name] = {'name': name, 'value': str(value), 'units': unit}

    def add_scatter(self, name, x, y, mode='lines+points', color='red', **kwargs):
        """ 
        See https://plot.ly/python/reference/#scatter for additional valid settings.
        Assumes incoming x, y data are numpy arrays.
        """
        self.output_figures[name] = {'name': name, 'type': 'scatter', 'x': x, 'y': y, 'mode': mode, 'marker':{'color': color},**kwargs}

    def add_surface(self, name, x, y, z, **kwargs):
        """
        See https://plot.ly/python/reference/#surface for additional valid settings.
        If you need an option there, just add another key-value pair.
        Assumes incoming x, y, z data are numpy arrays.
        """
        self.output_figures[name] = {'name': name, 'type': 'surface', 'x': x, 'y': y, 'z': z, **kwargs}

    def dump_to_console(self):
        """
        Dumps the cached outputs and graphs to the console. For regular values (e.g. a = 5),
        this is printed directly to console. Graphs are plotted via PyPlot and then displayed.
        """
        for name in self.output_dict:
            output = self.output_dict[name]
            print("{}: {} {}".format(name, output['value'], output['units']))

        for name, raw_fig in self.output_figures.items():
            raw_fig_type = raw_fig['type']
            if raw_fig_type == 'scatter':
                plt.figure()
                plt.title(name)
                x = raw_fig['x']
                y = raw_fig['y']
                marker = 'o'
                if 'xlabel' in raw_fig:
                    plt.xlabel(raw_fig['xlabel'])
                if 'ylabel' in raw_fig:
                    plt.ylabel(raw_fig['ylabel'])
                plt.plot(x, y, marker)

            elif raw_fig_type == 'surface':
                fig = plt.figure()
                fig.suptitle(name)
                ax = fig.gca(projection='3d')
                x = np.array(raw_fig['x'])
                y = np.array(raw_fig['y'])
                z = np.array(raw_fig['z'])
                X, Y = np.meshgrid(x, y)
                ax.plot_surface(X, Y, z, cmap=cm.coolwarm)

                if 'xlabel' in raw_fig:
                    ax.set_xlabel(raw_fig['xlabel'])
                if 'ylabel' in raw_fig:
                    ax.set_ylabel(raw_fig['ylabel'])
                if 'zlabel' in raw_fig:
                    ax.set_zlabel(raw_fig['zlabel'])

            else:
                print("Unsupported figure type: {}".format(raw_fig_type))

        plt.show()

    def to_dict(self):
        """
        Converts the cached data to a python dictionary for Flask to convert to JSON.
        Assumes incoming data are numpy arrays.
        """

        for fig in self.output_figures.values():
            if fig['type'] == 'scatter':
                fig['x'] = fig['x'].tolist()
                fig['y'] = fig['y'].tolist()
            elif fig['type'] == 'surface':
                fig['x'] = fig['x'].tolist()
                fig['y'] = fig['y'].tolist()
                fig['z'] = fig['z'].tolist()

        return {'outputs': list(self.output_dict.values()), 'outputGraphs': list(self.output_figures.values())}
