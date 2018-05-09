from flask import Flask, jsonify, render_template, request, redirect, url_for
from functools import reduce
from model import *
import string

app = Flask(__name__, static_folder='../frontend/build/static', template_folder='../frontend/build')

@app.route('/')
def get_home():
    return render_template('index.html')

@app.route('/api/run_forward_model', methods=['POST'])
def server_run_forward_model():
    data = request.get_json()

    outputCache = OutputCache()

    with open('_zygo_ascii_file.txt', 'w+') as f:
        f.write(data['zygo'].replace('\r\n', '\n'))

    mat_dict = reduce((lambda x, y: {**x, y['name']: y['value']}), data['material'], {})
    laser_dict = reduce((lambda x, y: {**x, y['name']: y['value']}), data['laser'])

    zygo = ZygoAsciiFile('_zygo_ascii_file.txt')
    material = Material(**mat_dict)
    laser = Laser(**laser_dict)
    config = ForwardModelConfig(melt_time_assumption='standard', surface_absorption_method='standard',
                show_code_settings=False, show_material_properties=False, show_general_figures=True,
                show_debugging_figures=True, show_console_output=False)
    #run_forward_config_model(zygo, material, laser, config, outputCache)    
    return jsonify(outputCache.to_dict())

if __name__ == '__main__':
    app.run(debug=True)