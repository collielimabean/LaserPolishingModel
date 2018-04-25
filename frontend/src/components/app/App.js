import React, { Component } from 'react';
import InputWindow from '../input-window/InputWindow';
import OutputWindow from '../output-window/OutputWindow';
import './App.css';


export default class App extends Component {
  material_values = [
    {name: "stc", value: 0, units: "-", comments: ""},
    {name: "mu", value: 0, units: "m^2/s", comments: ""},
    {name: "k", value: 0, units: "W/m-K", comments: ""},
    {name: "rho_c", value: 0, units: "J/m^3-K", comments: ""},
    {name: "rho", value: 0, units: "kg/m^3", comments: ""},
    {name: "T_m", value: 0, units: "K", comments: ""},
    {name: "T_b", value: 0, units: "K", comments: ""},
    {name: "absp", value: 0, units: "-", comments: ""},
  ];

  laser_values = [
    {name: "Beam Radius", value: 0, units: "μm", comments: ""},
    {name: "Pulse Duration", value: 0, units: "s", comments: ""},
    {name: "Average Power", value: 0, units: "W", comments: ""},
    {name: "Duty Cycle", value: 0, units: "%", comments: ""},
    {name: "Pulse Frequency", value: 0, units: "Hz", comments: ""},
  ];

  state = {
    zygoText: undefined,
    outputGraphs: [],
    outputs: [],
    computationRunning: false
  }

  startForwardModel() {
    let fetchOptions = {
      method: 'POST',
      headers: new Headers({'Content-Type': 'application/json'}),
      mode: 'same-origin',
    };

    let payload = {
      material: this.material_values,
      laser: this.laser_values,
      zygo: this.state.zygoText
    };
    
    if (!payload.zygo) {
      alert("Please load a Zygo file first.");
      return;
    }

    fetchOptions.body = JSON.stringify(payload);

    console.log(payload['material']);
    console.log(payload['laser']);
    
    let responseCallback = (response) => {
      if (!response.ok)
        throw new Error("" + response.statusText);
      return response.json();
    };

    let jsonCallback = (json) => {
        this.setState({
          computationRunning: false,
          outputs: json['outputs'],
          outputGraphs: json['outputGraphs']
        });
    };

    /**
     * The python backend should return the following keys:
     * outputs - for general key/value pairs
     * graphs - for all graphs
     */
    fetch("/api/run_forward_model", fetchOptions)
      .then((response) => responseCallback(response))
      .then((json) => jsonCallback(json))
      .catch((wat) => { console.log(wat); this.setState({computationRunning: false}); });

    this.setState({computationRunning: true});
  }
  
  render() {
    return (
      <div>
        <h1 style={{textAlign: "center"}}>Laser Polishing Simulator</h1>

        <div className="app-container">
          <div>
            <InputWindow
              materialParams={this.material_values}
              laserParams={this.laser_values}
              onZygoFileLoad={(zygoText) => { this.setState({zygoText}); }}/>
            <button 
              onClick={this.startForwardModel.bind(this)} 
              disabled={this.state.computationRunning} 
              className="startBtn">
              Start
            </button>
          </div>

          <div>
            <OutputWindow
              outputs={this.state.outputs}
              outputGraphs={this.state.outputGraphs}/>
          </div>
        </div>
      </div>
    );
  }
}
