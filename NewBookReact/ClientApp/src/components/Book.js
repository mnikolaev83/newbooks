import React, { Component } from 'react';
import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faCartPlus, faCartArrowDown, faBan, faHeart } from '@fortawesome/free-solid-svg-icons'

library.add(faCartPlus, faCartArrowDown, faBan, faHeart)

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

export class Book extends Component {

    addToCart = id => e => {
        fetch(`http://newbooksapi/api/wish_add?id=${id}`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.is_in_wishlist = true;
                this.setState(newState);
            });
    };
    removeFromCart = id => e => {
        fetch(`http://newbooksapi/api/wish_remove?id=${id}`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.is_in_wishlist = false;
                this.setState(newState);
            });
    };
    showIgnoreArea(event) {
        var newState = this.state;
        newState.hideIgnoreArea = false;
        this.setState(newState);
    };
    cancelIgnore(event) {
        var newState = this.state;
        newState.hideIgnoreArea = true;
        this.setState(newState);
    };
    commitIgnore(event) {
        var url = `http://newbooksapi/api/ignore?`;
        var ignoreData = this.state.ignoredData;
        if (ignoreData.categoryId !== undefined)
            if (ignoreData.categoryId > 0)
                url = url.concat(`category_id=${ignoreData.categoryId}&`);
        if (ignoreData.subcategory !== undefined) {
            var encodedSubcategory = encodeURIComponent(ignoreData.subcategory);
            url = url + "subcategory=" + encodedSubcategory + "&";
        }
        if (ignoreData.target !== undefined) {
            var encodedTarget = encodeURIComponent(ignoreData.target);
            url = url + "target=" + encodedTarget + "&";
        }
        if (ignoreData.series !== undefined) {
            var encodedSeries = encodeURIComponent(ignoreData.series);
            url = url + "series=" + encodedSeries + "&";
        }
        if (ignoreData.publisher !== undefined) {
            var encodedpublisher = encodeURIComponent(ignoreData.publisher);
            url = url + "publisher=" + encodedpublisher + "&";
        }
        fetch(url)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.hideIgnoreArea = true;
                this.setState(newState);
            });
    };
    constructor(props) {
        super(props);
        var categoryName = this.props.data.category_name;
        var categoryId = this.props.data.category_id;
        var publisher = this.props.data.publisher;
        if (publisher == null)
            publisher = "";
        var subcategory = this.props.data.subcategory;
        if (subcategory == null)
            subcategory = "";
        var series = this.props.data.series;
        if (series == null)
            series = "";
        var target = this.props.data.target;
        if (target == null)
            target = "";
        this.ignoredData = {
            categoryId: categoryId,
            categoryName: categoryName,
            subcategory: subcategory,
            target: target,
            publisher: publisher,
            series: series
        }
        this.state = {
            is_in_wishlist: this.props.data.is_in_wishlist,
            hideIgnoreArea: true,
            ignoredData: {
                categoryId: categoryId,
                categoryName: categoryName,
                subcategory: subcategory,
                target: target,
                publisher: publisher,
                series: series
            }
        };
    }

    changeIgnoreCategory(event) {
        var newState = this.state;
        newState.ignoredData.categoryId = event.target.value;
        this.setState(newState);
    }
    changeIgnoreSubcategory(event) {
        var newState = this.state;
        newState.ignoredData.subcategory = event.target.value;
        this.setState(newState);
    }
    changeIgnorePublisher(event) {
        var newState = this.state;
        newState.ignoredData.publisher = event.target.value;
        this.setState(newState);
    }
    changeIgnoreTarget(event) {
        var newState = this.state;
        newState.ignoredData.target = event.target.value;
        this.setState(newState);
    }
    changeIgnoreSeries(event) {
        var newState = this.state;
        newState.ignoredData.series = event.target.value;
        this.setState(newState);
    }

    render() {
        var categoryName = this.props.data.category_name;
        var categoryId = this.props.data.category_id;
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

        var bookclass = "row book";
        if (this.props.data.is_favorite)
            bookclass = "row book favorite";

        var cartPlusClass = "cart-plus left-icons"
        var cartMinusClass = "cart-minus left-icons"
        if (this.state.is_in_wishlist)
            cartPlusClass = "cart-plus left-icons hidden";
        else
            cartMinusClass = "cart-minus left-icons hidden";


        return (
            <div className={bookclass}>
                <div className={this.state.hideIgnoreArea ? 'hidden' : 'book-ignore'}>
                    <div className="popup-fade">
                    </div>
                    <div className="popup-book-ignore">
                        <div className="row">
                            <div className="col-sm-2">
                                Категория:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.ignoredData.categoryId} onChange={this.changeIgnoreCategory.bind(this)}>
                                    <option value={categoryId}>{categoryName}</option>
                                    <option value="">Все</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Подкатегория:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.ignoredData.subcategory} onChange={this.changeIgnoreSubcategory.bind(this)}>
                                    <option value={subcategory}>{subcategory}</option>
                                    <option value="">Все</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Назначение:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.ignoredData.target} onChange={this.changeIgnoreTarget.bind(this)}>
                                    <option value={target}>{target}</option>
                                    <option value="">Все</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Издательство:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.ignoredData.publisher} onChange={this.changeIgnorePublisher.bind(this)}>
                                    <option value={publisher}>{publisher}</option>
                                    <option value="">Все</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Серия:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.ignoredData.series} onChange={this.changeIgnoreSeries.bind(this)}>
                                    <option value={series}>{series}</option>
                                    <option value="">Все</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                <button onClick={this.commitIgnore.bind(this)}>
                                    Скрывать
                                </button>
                            </div>
                            <div className="col-sm-10">
                                <button onClick={this.cancelIgnore.bind(this)}>
                                    Отменить
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
                <div className="col-sm-2">
                    <div className="row">
                        <div className="col-sm-2">
                            <div>
                                {number}.
                            </div>
                            <div className={cartPlusClass} id={id} onClick={this.addToCart(id).bind(this)}>
                                <FontAwesomeIcon icon="cart-plus" />
                            </div>
                            <div className={cartMinusClass} id={id} onClick={this.removeFromCart(id).bind(this)}>
                                <FontAwesomeIcon icon="cart-arrow-down" />
                            </div>
                            <div className="man-buttons" onClick={this.showIgnoreArea.bind(this)}>
                                <div className="ban left-icons">
                                    <FontAwesomeIcon icon="ban" />
                                </div>
                                <div className="heart left-icons">
                                    <FontAwesomeIcon icon="heart" />
                                </div>
                            </div>
                        </div>
                        <div className="col-sm-10">
                            <img src={imageUrl} className="book_cover" />
                        </div>
                    </div>
                </div>
                <div className="col-sm-10">
                    <div className="row">
                        <div className="col-sm-12">
                            <span className="author-name">{authorName}</span>
                            <span className="book-title">{title}</span>
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
                            <span className="series">{series}</span>
                            <span className="isbn">ISBN: {isbn}</span>
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

export default Book;
