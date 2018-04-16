from flask import Flask, jsonify, render_template, request, redirect, url_for
from .model.forward_model import run_forward_model
from .model.forward_model_config import ForwardModelConfig
from .model.laser import Laser
from .model.material import Material
from .model.zygo import ZygoAsciiFile
from .model.output_cache import OutputCache
import string

app = Flask(__name__, static_folder='../frontend/build/static', template_folder='../frontend/build')

@app.route('/')
def get_home():
    return render_template('index.html')

@app.route('/api/run_forward_model', methods=['POST'])
def server_run_forward_model():
    data = request.get_json()
    return jsonify({"outputs": [{"name": "a", "value": 123, "units" :" -"}, {"name": "b", "value": 456.12, "units" :" -"}], "outputGraphs": [{"name": "g1"}, {"name": "g2"}]})
    # in reality...
    # outputCache = OutputCache()
    # run_forward_config_model(zygo, material, laser, outputCache)
    # return jsonify(outputCache.to_dict())


if __name__ == '__main__':
    app.run(debug=True)