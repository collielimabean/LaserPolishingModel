from flask import Flask, jsonify, render_template

app = Flask(__name__, static_folder='../frontend/build/static', template_folder='../frontend/build')

@app.route('/')
def get_home():
    return render_template('index.html')


if __name__ == '__main__':
    app.run(debug=True)