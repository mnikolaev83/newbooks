import React, { Component } from 'react';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCartPlus, faCartArrowDown, faBan, faHeart } from '@fortawesome/free-solid-svg-icons'

library.add(faCartPlus, faCartArrowDown, faBan, faHeart)

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

export class WishListItem extends Component {

    constructor(props) {
        super(props);
        this.state = {
            is_in_wishlist: true,
        };
    }
    removeFromWishList() {
        this.props.removeFromWishList(this.props.itemid);
    };
    render() {
        var categoryName = this.props.data.category_name;
        var id = this.props.data.id
        var number = this.props.number;
        var imageUrl = this.props.data.image_url;
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
        var target = this.props.data.target;
        if (target == null)
            target = "";
        target = target.replaceAll("&quot;", "'")
        var description = this.props.data.description;
        if (description == null)
            description = "";
        var translated = this.props.data.translated;
        if (translated == null)
            translated = "";
        translated = translated.replaceAll("&quot;", "'")
        description = description.replaceAll("&quot;", "'")
        var pages_amnt = this.props.data.pages_amnt;
        if (pages_amnt == null || pages_amnt == 0)
            pages_amnt = "";
        else
            pages_amnt = ", " + pages_amnt + " с.";
        var year = this.props.data.year;
        if (year == null || year === 0)
            year = "";
        else
            year = ", " + year + " г. ";
        var isbn = this.props.data.isbn;
        if (isbn == null)
            isbn = "";
        else
            isbn = "ISBN: " + isbn.replaceAll("&quot;", "'");
        return (
            <div className={this.state.is_in_wishlist ? "row wishlist-item" : "hidden"}>
                <div className={this.state.waitMode ? 'main-wait' : 'hide'}>
                </div>
                <div className="col-sm-2">
                    <div className="row">
                        <div className="col-sm-2">
                            <div>
                                {number}.
                            </div>
                            <div className="cart-minus left-icons" id={id} onClick={this.removeFromWishList.bind(this)}>
                                <FontAwesomeIcon icon="cart-arrow-down" />
                            </div>
                            <div className="man-buttons">
                            </div>
                        </div>
                        <div className="col-sm-10">
                            <img src={imageUrl} className="book_cover" />
                        </div>
                    </div>
                </div>
                <div className="col-sm-10">
                    <div className="category-name">
                        <span>{categoryName}</span>
                    </div>
                    <div className="row">
                        <div className="col-sm-12">
                            <span className="author-name">{authorName}</span>
                            <span className="book-title">{title}</span>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-sm-12">
                            <span className="translated">{translated}</span>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-sm-12">
                            <span className="subcategory">{subcategory}</span>
                            <span className="target">{target}</span>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-sm-12">
                            <span className="publisher">{publisher}</span>
                            <span className="year">{year}</span>
                            <span className="pages_amnt">{pages_amnt}</span>
                            <span className="series">{series}</span>
                            <span className="isbn">{isbn}</span>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-sm-12">
                            <span className="description">{description}</span>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default WishListItem;
