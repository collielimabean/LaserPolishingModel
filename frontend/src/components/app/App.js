import React, { Component } from 'react';
import {Grid, Row, Column} from 'react-cellblock';
import Plot from 'react-plotly.js';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import InputWindow from '../input-window/InputWindow';
import FileReaderInput from 'react-file-reader-input';
import './App.css';

export default class App extends Component {

  state = {
    file_name: null,
    file_contents: null,
    done: false
  }

  fileChange(e, results) {
    let result = results[0];
    let file = result[1];
    let reader = new FileReader();
    reader.onloadend = (e) => {
      if (e.target.readyState === FileReader.DONE) {
        let text = reader.result;
        this.setState({
          file_name: file.name,
          file_contents: text
        });
      }
    };
    
    reader.readAsText(file);
  }

  issueData() {
    let fetchOptions = {
      method: 'POST',
      headers: new Headers({'Content-Type': 'application/json'}),
      mode: 'same-origin',
    };

    let payload = {
      material: this.inputWindow.material_values,
      laser: this.inputWindow.laser_values,
      zygo: this.state.file_contents
    };

    if (!payload.zygo) {
      alert("Please load a Zygo file first.");
      return;
    }

    fetchOptions.body = JSON.stringify(payload);

    let responseCallback = (response) => {
      if (!response.ok) {
        console.log(response.statusText);
        return;
      }

      let data = JSON.parse(response.text());
      /**
       * structure: {
       *  input_graphs: [
       *    { name: "", data:""}
       * ],
       *   output_graphs: [
       *    { name:"", data:""}
       * ]
       * }
       */
      console.log(response.text());
      // TODO: parse and set graphs
    }

    fetch("/api/run_forward_model", fetchOptions)
      .then((response) => responseCallback(response));
  }
  
  render() {
    return (
      <div>
        <div>
          <h1>Laser Polishing Simulator</h1>
        </div>

        <div>
          <Grid gutterWidth={100}>
            <Row>
              <Column width="1/2">
              {
                this.state.file_name ? 
                (<p>Loaded {this.state.file_name}</p>) :
                    (<FileReaderInput as="text" onChange={this.fileChange.bind(this)}>
                      <button>Select Zygo File</button>
                    </FileReaderInput>)
              }
                <InputWindow ref={(iw) => { this.inputWindow = iw }} />
              </Column>
              <Column width="1/2">
                <Tabs>
                  <TabList>
                    <Tab>Original Surface</Tab>
                  </TabList>

                  <TabPanel>
                    <Plot data={[
                        {
                          type: 'surface',
                          x: [0, 1, 2, 3],
                          y: [0, 1, 2, 3],
                          z: [
                            [1, 1, 1, 1],
                            [2, 2, 2, 2],
                            [3, 3, 3, 3],
                            [4, 4, 4, 4]
                          ],
                          marker: {color: 'red'}
                        }
                      ]}/>
                  </TabPanel>
                </Tabs>
              </Column>
            </Row>
            <Row>
              <Column width="1/2">
                <button onClick={this.issueData.bind(this)}>Run</button>
              </Column>
              <Column width="1/2">
              </Column>
            </Row>
          </Grid>
        </div>
      </div>
    );
  }
}
