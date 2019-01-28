import React, { Component } from 'react';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'

library.add(faTrashAlt)

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

export class IgnoreListItem extends Component {
    constructor(props) {
        super(props);
        this.state = {
            data: this.props.data,
            isRemoved: false
        };
    }
    removeFromIgnoreList() {
        this.props.removeFromIgnoreList(this.props.itemid);
    };
    render() {
        var categoryName = this.props.data.category_name;
        if (categoryName == null || categoryName == '')
            categoryName = "[Все]";
        var id = this.props.data.id
        var publisher = this.props.data.publisher;
        if (publisher === null || publisher === '')
            publisher = "[Все]";
        publisher = publisher.replaceAll("&quot;", "'")
        var subcategory = this.props.data.subcategory;
        if (subcategory === null || subcategory === '')
            subcategory = "[Все]";
        subcategory = subcategory.replaceAll("&quot;", "'")
        var series = this.props.data.series;
        if (series === null || series === '')
            series = "[Все]";
        series = series.replaceAll("&quot;", "'")
        var target = this.props.data.target;
        if (target === null || target === '')
            target = "[Все]";
        target = target.replaceAll("&quot;", "'")
        return (
            <div className={this.state.isRemoved ? "hidden" : "table-list-item ignore row"}>
                <div className={this.state.waitMode ? 'main-wait' : 'hide'}>
                </div>
                <div className="col-sm-1 trash">
                    <FontAwesomeIcon icon="trash-alt" onClick={this.removeFromIgnoreList.bind(this)}/>
                </div>
                <div className="col-sm-2">
                    {categoryName}
                </div>
                <div className="col-sm-3">
                    {subcategory}
                </div>
                <div className="col-sm-3">
                    {target}
                </div>
                <div className="col-sm-1">
                    {publisher}
                </div>
                <div className="col-sm-2">
                    {series}
                </div>
            </div>
        );
    }
}

export default IgnoreListItem;
