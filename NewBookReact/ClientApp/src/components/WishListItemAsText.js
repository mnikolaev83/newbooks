import React, { Component } from 'react';

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

export class WishListItemAsText extends Component {

    constructor(props) {
        super(props);
        this.state = {
            is_in_wishlist: true,
        };
    }
    render() {
        var categoryName = this.props.data.category_name;
        var authorName = this.props.data.author_name;
        if (authorName == null)
            authorName = "";
        authorName = authorName.replaceAll("&quot;", "'")
        var title = this.props.data.title;
        if (title == null)
            title = "";
        title = title.replaceAll("&quot;", "'")
        var publisher = this.props.data.publisher;
        if (publisher == null)
            publisher = "";
        publisher = publisher.replaceAll("&quot;", "'")
        var subcategory = this.props.data.subcategory;
        if (subcategory == null)
            subcategory = "";
        subcategory = subcategory.replaceAll("&quot;", "'")
        var series = this.props.data.series;
        if (series == null)
            series = "";
        series = series.replaceAll("&quot;", "'")
        var isbn = this.props.data.isbn;
        if (isbn == null)
            isbn = "";
        isbn = isbn.replaceAll("&quot;", "'")
        var target = this.props.data.target;
        if (target == null)
            target = "";
        target = target.replaceAll("&quot;", "'")
        var description = this.props.data.description;
        if (description == null)
            description = "";
        description = description.replaceAll("&quot;", "'")
        return (
            <div className={this.state.is_in_wishlist ? "book" : "hidden"}>
                [{categoryName + "/" + subcategory}] {isbn} - {authorName}. {title}.
            </div>
        );
    }
}

export default WishListItemAsText;
