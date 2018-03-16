import React from 'react';
import ReactTable from 'react-table'

export default class ParameterTable extends React.Component {
  COLUMNS = [
    { Header: 'Name', accessor: 'name' },
    { Header: 'Value', accessor: 'value' },
    { Header: 'Units', accessor: 'units' },
    { Header: 'Comment', accessor: 'comments' },
  ];

  componentDidMount() {
  }

  render() {
    return (
      <ReactTable
        data={this.props.parameters}
        columns={this.COLUMNS}
        defaultPageSize="5"
        />
    );
  }
} 
