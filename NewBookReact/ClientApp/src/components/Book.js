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
    showFavoriteArea(event) {
        var newState = this.state;
        newState.hideFavoriteArea = false;
        this.setState(newState);
    };
    cancelFavorite(event) {
        var newState = this.state;
        newState.hideFavoriteArea = true;
        this.setState(newState);
    };
    commitIgnore(event) {
        var currentState = this.state;
        currentState.waitMode = true;
        currentState.hideIgnoreArea = true;
        this.setState(currentState);
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
                newState.waitMode = false;
                this.setState(newState);
            });
    };
    commitFavorite(event) {
        var currentState = this.state;
        currentState.waitMode = true;
        currentState.hideFavoriteArea = true;
        this.setState(currentState);
        var url = `http://newbooksapi/api/add_to_favorite?`;
        var favoriteData = this.state.favoriteData;
        if (favoriteData.categoryId !== undefined)
            if (favoriteData.categoryId > 0)
                url = url.concat(`category_id=${favoriteData.categoryId}&`);
        if (favoriteData.subcategory !== undefined) {
            var encodedSubcategory = encodeURIComponent(favoriteData.subcategory);
            url = url + "subcategory=" + encodedSubcategory + "&";
        }
        if (favoriteData.target !== undefined) {
            var encodedTarget = encodeURIComponent(favoriteData.target);
            url = url + "target=" + encodedTarget + "&";
        }
        if (favoriteData.series !== undefined) {
            var encodedSeries = encodeURIComponent(favoriteData.series);
            url = url + "series=" + encodedSeries + "&";
        }
        if (favoriteData.publisher !== undefined) {
            var encodedpublisher = encodeURIComponent(favoriteData.publisher);
            url = url + "publisher=" + encodedpublisher + "&";
        }
        fetch(url)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.waitMode = false;
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
        this.state = {
            hideIgnoreArea: true,
            hideFavoriteArea: true,
            ignoredData: {
                categoryId: categoryId,
                categoryName: categoryName,
                subcategory: subcategory,
                target: target,
                publisher: publisher,
                series: series
            },
            favoriteData: {
                categoryId: categoryId,
                categoryName: categoryName,
                subcategory: subcategory,
                target: target,
                publisher: publisher,
                series: series
            }
        };
    }
    addToWishList() {
        this.props.addToWishList(this.props.itemid);
    };
    removeFromWishList() {
        this.props.removeFromWishList(this.props.itemid);
    };
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
    changeFavoriteCategory(event) {
        var newState = this.state;
        newState.favoriteData.categoryId = event.target.value;
        this.setState(newState);
    }
    changeFavoriteSubcategory(event) {
        var newState = this.state;
        newState.favoriteData.subcategory = event.target.value;
        this.setState(newState);
    }
    changeFavoritePublisher(event) {
        var newState = this.state;
        newState.favoriteData.publisher = event.target.value;
        this.setState(newState);
    }
    changeFavoriteTarget(event) {
        var newState = this.state;
        newState.favoriteData.target = event.target.value;
        this.setState(newState);
    }
    changeFavoriteSeries(event) {
        var newState = this.state;
        newState.favoriteData.series = event.target.value;
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
        var translated = this.props.data.translated;
        if (translated == null)
            translated = "";
        translated = translated.replaceAll("&quot;", "'")
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
        description = description.replaceAll("&quot;", "'")
        var bookclass = "row book";
        if (this.props.data.is_favorite)
            bookclass = "row book favorite";
        var cartPlusClass = "cart-plus left-icons"
        var cartMinusClass = "cart-minus left-icons"
        if (this.props.is_in_wishlist)
            cartPlusClass = "cart-plus left-icons hidden";
        else
            cartMinusClass = "cart-minus left-icons hidden";


        return (
            <div className={bookclass}>
                <div className={this.state.waitMode ? 'main-wait' : 'hide'}>
                </div>
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
                                    <option value="">[Все]</option>
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
                                    <option value="">[Все]</option>
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
                                    <option value="">[Все]</option>
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
                                    <option value="">[Все]</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Серия:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.ignoredData.series} onChange={this.changeIgnoreSeries.bind(this)}>
                                    <option value={series}>{series}</option>
                                    <option value="">[Все]</option>
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
                <div className={this.state.hideFavoriteArea ? 'hidden' : 'book-favorite'}>
                    <div className="popup-fade">
                    </div>
                    <div className="popup-book-favorite">
                        <div className="row">
                            <div className="col-sm-2">
                                Категория:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.favoriteData.categoryId} onChange={this.changeFavoriteCategory.bind(this)}>
                                    <option value={categoryId}>{categoryName}</option>
                                    <option value="">[Все]</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Подкатегория:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.favoriteData.subcategory} onChange={this.changeFavoriteSubcategory.bind(this)}>
                                    <option value={subcategory}>{subcategory}</option>
                                    <option value="">[Все]</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Назначение:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.favoriteData.target} onChange={this.changeFavoriteTarget.bind(this)}>
                                    <option value={target}>{target}</option>
                                    <option value="">[Все]</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Издательство:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.favoriteData.publisher} onChange={this.changeFavoritePublisher.bind(this)}>
                                    <option value={publisher}>{publisher}</option>
                                    <option value="">[Все]</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                Серия:
                            </div>
                            <div className="col-sm-10">
                                <select value={this.state.favoriteData.series} onChange={this.changeFavoriteSeries.bind(this)}>
                                    <option value={series}>{series}</option>
                                    <option value="">[Все]</option>
                                </select>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-sm-2">
                                <button onClick={this.commitFavorite.bind(this)}>
                                    Отмечать
                                </button>
                            </div>
                            <div className="col-sm-10">
                                <button onClick={this.cancelFavorite.bind(this)}>
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
                            <div className={cartPlusClass} id={id} onClick={this.addToWishList.bind(this)}>
                                <FontAwesomeIcon icon="cart-plus" />
                            </div>
                            <div className={cartMinusClass} id={id} onClick={this.removeFromWishList.bind(this)}>
                                <FontAwesomeIcon icon="cart-arrow-down" />
                            </div>
                            <div className="man-buttons">
                                <div className="ban left-icons" onClick={this.showIgnoreArea.bind(this)}>
                                    <FontAwesomeIcon icon="ban" />
                                </div>
                                <div className="heart left-icons" onClick={this.showFavoriteArea.bind(this)}>
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
                            <span className="year"> {year}</span>
                            <span className="pages_amnt"> {pages_amnt}</span>
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

export default Book;
