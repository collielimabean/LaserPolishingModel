import React, { Component } from 'react';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import ReactTable from 'react-table';
import Plot from 'react-plotly.js'

//import Plotly from '../../util/bundle';
//import createPlotlyComponent from 'react-plotly.js/factory';
//const Plot = createPlotlyComponent(Plotly);

export default class OutputWindow extends Component {
  OUTPUT_COLUMNS = [
    { Header: 'Output Name', accessor: 'name' },
    { Header: 'Value', accessor: 'value' },
    { Header: 'Units', accessor: 'units' },
  ];

  render() {
    return (
      <div>
        <Tabs>
          <TabList>
            <Tab>Output</Tab>
            { this.props.outputGraphs.map((og) => { return <Tab>{og.name}</Tab> }) }
            </TabList>

            <TabPanel>
              <ReactTable         
                data={this.props.outputs}
                columns={this.OUTPUT_COLUMNS}
                defaultPageSize={10}/>
            </TabPanel>

            { 
              this.props.outputGraphs.map((og) => {
                return (
                  <TabPanel>
                    <Plot data={[og]} layout={{title:og.name}}/>
                  </TabPanel>
                );
              })
            }
        </Tabs>
      </div>
    );
  }
}
