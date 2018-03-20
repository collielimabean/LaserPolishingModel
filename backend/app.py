from flask import Flask, jsonify, render_template, request, redirect, url_for

app = Flask(__name__, static_folder='../frontend/build/static', template_folder='../frontend/build')

@app.route('/')
def get_home():
    return render_template('index.html')

@app.route('/api/run_forward_model')
def run_forward_model():
    pass

if __name__ == '__main__':
    app.run(debug=True)