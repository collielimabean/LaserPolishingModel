import React from 'react';
import ReactTable from 'react-table'

/**
 * An editable table to handle parameters.
 * @class
 */
export default class ParameterTable extends React.Component {
  COLUMNS = [
    { Header: 'Name', accessor: 'name' },
    { Header: 'Value', accessor: 'value', Cell: this.renderEditable.bind(this) },
    { Header: 'Units', accessor: 'units' },
    { Header: 'Comment', accessor: 'comments', Cell: this.renderEditable.bind(this) },
  ];

  constructor(props) {
    super(props);
    this.state = { data: this.props.parameters };
  }

  renderEditable(cellInfo) {
    return (
      <div
        style={{ backgroundColor: "#fafafa" }}
        contentEditable
        suppressContentEditableWarning
        onBlur={e => {
          const data = [...this.state.data];
          let leafNode = e.target;
          while (leafNode.children.length > 0) {
            leafNode = leafNode.childNodes[0];
          }

          data[cellInfo.index][cellInfo.column.id] = leafNode.innerHTML;
          this.setState({ data });
        }}
        dangerouslySetInnerHTML={{
          __html: this.state.data[cellInfo.index][cellInfo.column.id]
        }}
      />
    );
  }

  componentDidMount() {
  }

  /** @override */
  render() {
    return (
      <ReactTable
        data={this.props.parameters}
        columns={this.COLUMNS}
        defaultPageSize={10}
        />
    );
  }
} 
