from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt
import numpy as np
import json

class OutputCache:
    def __init__(self):
        self.output_dict = {}
        self.output_figures = []

    def add_output(self, name, value, unit):
        self.output_dict[name] = {'name': name, 'value': value, 'units': unit}

    def add_scatter(self, name, x, y, mode='markers', **kwargs):
        """ 
        See https://plot.ly/python/reference/#scatter for additional valid settings.
        """
        self.output_figures[name] = {'type': 'scatter', 'x': x, 'y': y, 'mode': mode, **kwargs}

    def add_surface(self, name, x, y, z, **kwargs):
        """
        See https://plot.ly/python/reference/#surface for additional valid settings.
        If you need an option there, just add another key-value pair.
        """
        self.output_figures[name] = {'type': 'scatter', 'x': x, 'y': y, 'z': z, **kwargs}

    def dump_to_console(self):
        for name in self.output_dict:
            output = self.output_dict[name]
            print("{}: {} {}".format(name, output['value'], output['units']))

        for raw_fig in self.output_figures:
            raw_fig_type = raw_fig['type']
            if raw_fig_type == 'scatter':
                plt.figure()
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
                ax = fig.add_subplot(111, projection='3d')
                x = np.array(raw_fig['x'])
                y = np.array(raw_fig['y'])
                z = np.array(raw_fig['z'])
                X, Y = np.meshgrid(x, y)
                ax.plot_surface(X, Y, z)

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
        return {'outputs': self.output_dict.values(), 'outputGraphs': self.output_figures}