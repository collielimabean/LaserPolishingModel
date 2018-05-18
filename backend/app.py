"""
app.py is the main entry for the backend python server.
"""

from flask import Flask, jsonify, render_template, request, redirect, url_for
from functools import reduce
from model import *
import string

# instruct Flask, the python server, to serve the frontend javascript code
# at the specified relative path to this file.
app = Flask(__name__, static_folder='../frontend/build/static', template_folder='../frontend/build')

@app.route('/')
def get_home():
    """ When the user accesses the website root, return the frontend home page."""
    return render_template('index.html')

@app.route('/api/run_forward_model', methods=['POST'])
def server_run_forward_model():
    """ 
    When the frontend issues a HTTP POST, read the given data, and 
    run the forward model.

    The received JSON data is in the following format:
    {
        material: [
            { name: string, value: double, unit: string, comments: string },
            ...
        ],
        laser: [
            { name: string, value: double, unit: string, comments: string },
            ...
        ],
        zygo: string (the raw zygo ASCII file text)
    }

    Note: the data transfer from the frontend to the backend may take a while.

    Returns a dictionary of the forward model results. See the documentation of the
    OutputCache::to_dict() method.
    """
    data = request.get_json()

    # holds all eventual outputs, both graphical and value
    outputCache = OutputCache()

    # save the zygo file in a temp file
    with open('_zygo_ascii_file.txt', 'w+') as f:
        f.write(data['zygo'].replace('\r\n', '\n'))

    # convert the frontend material and laser parameter data into the backend format
    mat_dict = reduce((lambda x, y: {**x, y['name']: y['value']}), data['material'], {})
    laser_dict = reduce((lambda x, y: {**x, y['name']: y['value']}), data['laser'])

    # initialize the forward model configuration and settings
    zygo = ZygoAsciiFile('_zygo_ascii_file.txt')
    material = Material(**mat_dict)
    laser = Laser(**laser_dict)
    config = ForwardModelConfig(melt_time_assumption='standard', surface_absorption_method='standard',
                show_code_settings=False, show_material_properties=False, show_general_figures=True,
                show_debugging_figures=True, show_console_output=False)

    # run the model, and return the output
    run_forward_config_model(zygo, material, laser, config, outputCache)    
    return jsonify(outputCache.to_dict())

if __name__ == '__main__':
    app.run(debug=True)