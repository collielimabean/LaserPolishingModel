import React, { Component } from 'react';
import InputWindow from '../input-window/InputWindow';
import OutputWindow from '../output-window/OutputWindow';
import './App.css';

/** 
 * The main window, layout, and controller for the frontend. 
 * @class
 */
export default class App extends Component {
  /** Default material values and units. */
  material_values = [
    {name: "stc", value: -0.48e-3, units: "-", comments: ""},
    {name: "mu", value: 5.5e-3, units: "m^2/s", comments: ""},
    {name: "k", value: 28.5, units: "W/m-K", comments: ""},
    {name: "rho_c", value: 6.05e6, units: "J/m^3-K", comments: ""},
    {name: "rho", value: 7783, units: "kg/m^3", comments: ""},
    {name: "T_m", value: 1727, units: "K", comments: ""},
    {name: "T_b", value: 3134, units: "K", comments: ""},
    {name: "absp", value: 0.38, units: "-", comments: ""},
  ];

  /** Default laser parameters values and units. */
  laser_values = [
    {name: "Beam Radius", value: 15e-6, units: "μm", comments: ""},
    {name: "Pulse Duration", value: 3e-6, units: "s", comments: ""},
    {name: "Average Power", value: 3, units: "W", comments: ""},
    {name: "Duty Cycle", value: 0.25, units: "%", comments: ""},
    {name: "Pulse Frequency", value: 20e3, units: "Hz", comments: ""},
  ];

  state = {
    zygoText: undefined,
    outputGraphs: [],
    outputs: [],
    computationRunning: false
  }

  /**
   * This method is called when the start button is clicked. 
   * This method is responsible for issuing the HTTP POST to the backend. 
   */
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
  
  /** @override */
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
