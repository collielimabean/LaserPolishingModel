import React, { Component } from 'react';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import ParameterTable from '../parameter-table/ParameterTable';
import Dropzone from 'react-dropzone';
import './InputWindow.css';

export default class InputWindow extends Component {
  state = {
    fileName: undefined
  }

  uploadZygoFile(file) {
    const reader = new FileReader();
    reader.onload = (e) => { 
      this.props.onZygoFileLoad(reader.result);
      this.setState({fileName: file[0].name })
    };
    reader.readAsText(file[0]);
  }

  render() {
    return (
      <div className="iw-container">
        <div>
          <Dropzone onDrop={this.uploadZygoFile.bind(this)} className="zygo-dropzone">
            <div className="zygo-dropzone-center">
              { this.state.fileName ? 
                  <h3>Loaded file {this.state.fileName}</h3> :
                  <h3>Click or drag a file...</h3> }
            </div>
          </Dropzone>
        </div>
        <div>
          <Tabs>
            <TabList>
              <Tab>Material</Tab>
              <Tab>Laser</Tab>
            </TabList>

            <TabPanel>
              <ParameterTable parameters={this.props.materialParams}/>
            </TabPanel>
            <TabPanel>
              <ParameterTable parameters={this.props.laserParams}/>
            </TabPanel>
          </Tabs>
        </div>
      </div>
    );
  }
}
