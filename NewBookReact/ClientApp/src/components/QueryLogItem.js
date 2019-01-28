import React, { Component } from 'react';

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

export class QueryLogItem extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.props.data
        };
    }
    render() {
        return (
            <div className={this.props.even ? "even-row table-list-item row" : "table-list-item row"}>
                <div className={this.state.waitMode ? 'main-wait' : 'hide'}>
                </div>
                <div className="col-sm-3">
                    {this.props.data.query_at}
                </div>
                <div className="col-sm-3">
                    {this.props.data.category_name}
                </div>
                <div className="col-sm-3">
                    {this.props.data.period}
                </div>
                <div className="col-sm-3">
                    {this.props.data.books_fetched}
                </div>
            </div>
        );
    }
}

export default QueryLogItem;
