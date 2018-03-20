import React, { Component } from 'react';
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs';
import ParameterTable from '../parameter-table/ParameterTable';

export default class InputWindow extends Component {
  material_values = [
    {name: "stc", value: 0, units: "-", comments: ""},
    {name: "mu", value: 0, units: "m^2/s", comments: ""},
    {name: "alpha", value: 0, units: "W/m-K", comments: ""},
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

  render() {
    return (
      <Tabs>
        <TabList>
          <Tab>Material</Tab>
          <Tab>Laser</Tab>
        </TabList>

        <TabPanel>
          <ParameterTable parameters={this.material_values}/>
        </TabPanel>
        <TabPanel>
          <ParameterTable parameters={this.laser_values}/>
        </TabPanel>
      </Tabs>
    );
  }
}
