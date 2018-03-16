import React, { Component } from 'react';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import ParameterTable from '../parameter-table/ParameterTable';
import ReactTable from 'react-table';

export default class InputWindow extends Component {
  render() {
    return (
      <Tabs>
        <TabList>
          <Tab>Material</Tab>
          <Tab>Laser</Tab>
        </TabList>

        <TabPanel>
          <ParameterTable/>
        </TabPanel>
        <TabPanel>
          <ParameterTable/>
        </TabPanel>
      </Tabs>
    );
  }
}
