import React, { Component } from 'react';

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

export class JobLogItem extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.props.data
        };
    }
    render() {
        var className = "table-list-item row";
        if (this.props.even)
            className = className + " even-row";
        if (this.props.data.error_occured)
            className = className + " error";

        return (
            <div className={className}>
                <div className={this.state.waitMode ? 'main-wait' : 'hide'}>
                </div>
                <div className="col-sm-4">
                    {this.props.data.started_at}
                </div>
                <div className="col-sm-4">
                    {this.props.data.error_occured ? "ОШИБКА!" : this.props.data.completed_at}
                </div>
                <div className="col-sm-4">
                    {this.props.data.books_fetched}
                </div>
            </div>
        );
    }
}

export default JobLogItem;
