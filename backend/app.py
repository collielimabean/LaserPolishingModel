from flask import Flask, jsonify, render_template, request, redirect, url_for
import string

app = Flask(__name__, static_folder='../frontend/build/static', template_folder='../frontend/build')

@app.route('/')
def get_home():
    return render_template('index.html')

@app.route('/api/run_forward_model', methods=['POST'])
def run_forward_model():
    data = request.get_json()
    return jsonify({"outputs": [{"name": "a", "value": 123, "units" :" -"}, {"name": "b", "value": 456.12, "units" :" -"}], "outputGraphs": [{"name": "g1"}, {"name": "g2"}]})

if __name__ == '__main__':
    app.run(debug=True)