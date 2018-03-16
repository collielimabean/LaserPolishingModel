import React, { Component } from 'react';
import {Grid, Row, Column} from 'react-cellblock';
import Plot from 'react-plotly.js';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import InputWindow from '../input-window/InputWindow';

import './App.css';

export default class App extends Component {
  componentDidMount() {
    document.title = 'Laser Polishing Simulator';
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
                <InputWindow />
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
