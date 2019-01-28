import React, { Component } from 'react';
import './Home.css';
import DatePicker from "react-datepicker";
import Book from "./Book";
import WishListItem from "./WishListItem";
import IgnoreListItem from "./IgnoreListItem";
import FavoriteListItem from "./FavoriteListItem";
import WishListItemAsText from "./WishListItemAsText";
import QueryLogItem from "./QueryLogItem";
import JobLogItem from "./JobLogItem";
import { registerLocale, setDefaultLocale } from "react-datepicker";
import ru from 'date-fns/locale/ru';
import "react-datepicker/dist/react-datepicker.css";
import dateFormat from "dateformat";
registerLocale('ru', ru);
setDefaultLocale('ru');

export class Home extends Component {

    getPreviousMonday() {
        var date = new Date();
        var day = date.getDay();
        var prevMonday;
        prevMonday = new Date().setDate(date.getDate() - day - 7 + 1);
        return new Date(prevMonday);
    }
    getLastSunday() {
        var date = new Date();
        var day = date.getDay();
        var prevSunday;
        prevSunday = new Date().setDate(date.getDate() - day - 7 + 7);
        return new Date(prevSunday);
    }
    static displayName = Home.name;
    componentDidMount() {
        document.title = "Книжные новинки"
    }
    constructor() {
        super();
        var dateFrom = this.getPreviousMonday();
        var dateTo = this.getLastSunday();
        this.state = {
            categoryId: 0,
            categories: [],
            dateFrom: dateFrom,
            dateTo: dateTo,
            books: [],
            booksShown: 0,
            booksIgnored: 0,
            booksFavorite: 0,
            view: 'newbooks',
            showWishlistAsText: false,
            wishlist: [],
            ignorelist: [],
            favoritelist: [],
            querylog: [],
            joblog: []
        };
        fetch('http://newbooksapi/api/categories')
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.categories = data;
                newState.categoryId = data[0].id;
                this.setState(newState);
            });
        this.wishlistUpdate();
        this.ignorelistUpdate();
        this.favoritelistUpdate();
        this.queryLogUpdate();
        this.jobLogUpdate();
    }
    handleWishListModeChange(event) {
        var newState = this.state;
        newState.showWishlistAsText = event.target.checked;
        this.setState(newState);
    }
    handleCategoryChange(event) {
        var newState = this.state;
        newState.categoryId = event.target.value;
        this.setState(newState);
    }
    handleDateFromChange(date) {
        var newState = this.state;
        newState.dateFrom = date;
        this.setState(newState);
    }
    handleDateToChange(date) {
        var newState = this.state;
        newState.dateTo = date;
        this.setState(newState);
    }
    changeView = view => e => {
        var newState = this.state;
        newState.view = view;
        this.setState(newState);
        if (view === 'wishlist') {
            this.wishlistUpdate();
        } else if (view === 'ignorelist') {
            this.ignorelistUpdate();
        } else if (view === 'favoritelist') {
           this.favoritelistUpdate();
        } else if (view === 'querylog') {
            this.queryLogUpdate();
        } else if (view === 'joblog') {
            this.jobLogUpdate();
        }
    }
    newBooksUpdate(event) {
        var encodedDateFrom = encodeURIComponent(dateFormat(this.state.dateFrom, "yyyy-mm-dd"));
        var encodedDateTo = encodeURIComponent(dateFormat(this.state.dateTo, "yyyy-mm-dd"));
        var currentState = this.state;
        currentState.waitMode = true;
        this.setState(currentState);
        fetch(`http://newbooksapi/api/list?category_id=${this.state.categoryId}&date_added_from=${encodedDateFrom}&date_added_to=${encodedDateTo}`)
            .then(response => response.json())
            .then(data => {
                var booksShownCounter = 0;
                var booksIgnoredCounter = 0;
                var booksFavoriteCounter = 0;
                for (let i = 0; i < data.length; i++) {
                    if (!data[i].is_ignored) {
                        booksShownCounter++;
                    } else {
                        booksIgnoredCounter++;
                    }
                    if (data[i].is_favorite) {
                        booksFavoriteCounter++;
                    }
                }
                var newState = this.state;
                newState.books = data;
                newState.booksIgnored = booksIgnoredCounter;
                newState.booksShown = booksShownCounter;
                newState.booksFavorite = booksFavoriteCounter;
                newState.waitMode = false;
                for (var i = 0; i < this.state.categories.length; i++) {
                    if (parseInt(this.state.categories[i].id, 10) === parseInt(this.state.categoryId, 10)) {
                        newState.categoryName = this.state.categories[i].category_name;
                    }
                }
                this.setState(newState);
            });
    }
    wishlistUpdate() {
        var currentState = this.state;
        currentState.waitMode = true;
        this.setState(currentState);
        fetch(`http://newbooksapi/api/wishlist`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.wishlist = data;
                newState.waitMode = false;
                this.setState(newState);
            });
    }
    ignorelistUpdate() {
        var currentState = this.state;
        currentState.waitMode = true;
        this.setState(currentState);
        fetch(`http://newbooksapi/api/ignored`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.ignorelist = data;
                newState.waitMode = false;
                this.setState(newState);
            });
    }
    favoritelistUpdate() {
        var currentState = this.state;
        currentState.waitMode = true;
        this.setState(currentState);
        fetch(`http://newbooksapi/api/favorites`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.favoritelist = data;
                newState.waitMode = false;
                this.setState(newState);
            });
    }
    queryLogUpdate() {
        var currentState = this.state;
        currentState.waitMode = true;
        this.setState(currentState);
        fetch(`http://newbooksapi/api/query_log`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.querylog = data;
                newState.waitMode = false;
                this.setState(newState);
            });
    }
    jobLogUpdate() {
        var currentState = this.state;
        currentState.waitMode = true;
        this.setState(currentState);
        fetch(`http://newbooksapi/api/job_log`)
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.joblog = data;
                newState.waitMode = false;
                this.setState(newState);
            });
    }
    removeFromWishList(id) {
        if (window.confirm("Уверен?")) {
            fetch(`http://newbooksapi/api/wish_remove?id=${id}`)
                .then(response => response.json())
                .then(data => {
                    if (this.state !== null && this.state !== undefined) {
                        var newState = this.state;
                        for (var j = 0; j < newState.books.length; j++) {
                            if (newState.books[j].id === id) {
                                newState.books[j].is_in_wishlist = false;
                            }
                        }
                        for (j = 0; j < newState.wishlist.length; j++) {
                            if (newState.wishlist[j].id === id) {
                                newState.wishlist[j].is_removed = true;
                            }
                        }
                        this.setState(newState);
                    }
                });
        }
    }
    removeFromIgnoreList(id) {
        if (window.confirm("Уверен?")) {
            fetch(`http://newbooksapi/api/ignore_remove?id=${id}`)
                  .then(response => response.json())
                  .then(data => {
                      if (this.state !== null && this.state !== undefined) {
                          var newState = this.state;
                          for (var j = 0; j < newState.ignorelist.length; j++) {
                              if (newState.ignorelist[j].id === id) {
                                  newState.ignorelist[j].is_removed = true;
                              }
                          }
                          this.setState(newState);
                      }
                  });
        }
    }
    removeFromFavoriteList(id) {
        if (window.confirm("Уверен?")) {
            fetch(`http://newbooksapi/api/favorites_remove?id=${id}`)
                .then(response => response.json())
                .then(data => {
                    if (this.state !== null && this.state !== undefined) {
                        var newState = this.state;
                        for (var j = 0; j < newState.favoritelist.length; j++) {
                            if (newState.favoritelist[j].id === id) {
                                newState.favoritelist[j].is_removed = true;
                            }
                        }
                        this.setState(newState);
                    }
                });
        }
    }
    addToWishList(id) {
        fetch(`http://newbooksapi/api/wish_add?id=${id}`)
            .then(response => response.json())
            .then(data => {
                if (this.state !== null && this.state !== undefined) {
                    var newState = this.state;
                    for (var j = 0; j < newState.books.length; j++) {
                        if (newState.books[j].id === id) {
                            newState.books[j].is_in_wishlist = true;
                        }
                    }
                    for (j = 0; j < newState.wishlist.length; j++) {
                        if (newState.wishlist[j].id === id) {
                            newState.wishlist[j].is_removed = false;
                        }
                    }
                    this.setState(newState);
                }
            });
    }
    createBooks() {
        let books = [];
        var counter = 0;
        for (let i = 0; i < this.state.books.length; i++) {
            if (!this.state.books[i].is_ignored) {
                books.push(
                    <Book itemid={this.state.books[i].id} addToWishList={this.addToWishList.bind(this)} removeFromWishList={this.removeFromWishList.bind(this)} key={this.state.books[i].id} data={this.state.books[i]} number={counter + 1} is_in_wishlist={this.state.books[i].is_in_wishlist} />
                )
                counter++;
            }
        }
        return books;
    }
    createIgnoreList() {
        let list = [];
        list.push(
            <div className="table-list-item row header ignore">
                <div className="col-sm-1">
                </div>
                <div className="col-sm-2">
                    Категория
                </div>
                <div className="col-sm-3">
                    Подкатетегория
                </div>
                <div className="col-sm-3">
                    Целевое назначение
                </div>
                <div className="col-sm-1">
                    Издательство
                </div>
                <div className="col-sm-2">
                    Серия
                </div>
            </div>
        );
        for (let i = 0; i < this.state.ignorelist.length; i++) {
            if (!this.state.ignorelist[i].is_removed) {
                list.push(
                    <IgnoreListItem itemid={this.state.ignorelist[i].id} removeFromIgnoreList={this.removeFromIgnoreList.bind(this)} key={this.state.ignorelist[i].id} data={this.state.ignorelist[i]} />
                )
            }
        }
        return list;
    }
    createFavoriteList() {
        let list = [];
        list.push(
            <div className="table-list-item row header favorite">
                <div className="col-sm-1">
                </div>
                <div className="col-sm-2">
                    Категория
                </div>
                <div className="col-sm-3">
                    Подкатетегория
                </div>
                <div className="col-sm-3">
                    Целевое назначение
                </div>
                <div className="col-sm-1">
                    Издательство
                </div>
                <div className="col-sm-2">
                    Серия
                </div>
            </div>
        );
        for (let i = 0; i < this.state.favoritelist.length; i++) {
            if (!this.state.favoritelist[i].is_removed) {
                list.push(
                    <FavoriteListItem itemid={this.state.favoritelist[i].id} removeFromFavoriteList={this.removeFromFavoriteList.bind(this)} key={this.state.favoritelist[i].id} data={this.state.favoritelist[i]} />
                )
            }
        }
        return list;
    }
    createWishList() {
        let list = [];
        for (let i = 0; i < this.state.wishlist.length; i++) {
            if (!this.state.wishlist[i].is_removed) {
                list.push(
                    <WishListItem itemid={this.state.wishlist[i].id} removeFromWishList={this.removeFromWishList.bind(this)} key={this.state.wishlist[i].id} data={this.state.wishlist[i]} />
                )
            }
        }
        return list;
    }
    createWishListAsText() {
        var books = [];
        for (let i = 0; i < this.state.wishlist.length; i++) {
            if (!this.state.wishlist[i].is_removed) {
                books.push(
                    <WishListItemAsText data={this.state.wishlist[i]} />
                )
            }
        }
        return books;
    }
    createQueryLog() {
        let list = [];
        list.push(
            <div className="table-list-item row header">
                <div className="col-sm-3">
                    Дата / время
                </div>
                <div className="col-sm-3">
                    Категория
                </div>
                <div className="col-sm-3">
                    Период
                </div>
                <div className="col-sm-2">
                    Книг выбрано
                </div>
            </div>
        );
        for (let i = 0; i < this.state.querylog.length; i++) {
            list.push(
                <QueryLogItem even={i % 2 == 0 ? true : false} key={this.state.querylog[i].id} data={this.state.querylog[i]} />
                )
        }
        return list;
    }
    createJobLog() {
        let list = [];
        list.push(
            <div className="table-list-item row header">
                <div className="col-sm-4">
                    Начало
                </div>
                <div className="col-sm-4">
                    Завершение
                </div>
                <div className="col-sm-4">
                    Новых книг
                </div>
            </div>
        );
        for (let i = 0; i < this.state.joblog.length; i++) {
            list.push(
                <JobLogItem even={i % 2 == 0 ? true : false} key={this.state.joblog[i].id} data={this.state.joblog[i]} />
            )
        }
        return list;
    }
    getWishListLength() {
        if (this.state.wishlist === undefined || this.state.wishlist === null)
            return 0;
        var counter = 0;
        for (let i = 0; i < this.state.wishlist.length; i++) {
            if (!this.state.wishlist[i].is_removed) {
                counter++;
            }
        }
        return counter;
    }
    getIgnoreListLength() {
        if (this.state.ignorelist === undefined || this.state.ignorelist === null)
            return 0;
        var counter = 0;
        for (let i = 0; i < this.state.ignorelist.length; i++) {
            if (!this.state.ignorelist[i].is_removed) {
                counter++;
            }
        }
        return counter;
    }
    getFavoriteListLength() {
        if (this.state.favoritelist === undefined || this.state.favoritelist === null)
            return 0;
        var counter = 0;
        for (let i = 0; i < this.state.favoritelist.length; i++) {
            if (!this.state.favoritelist[i].is_removed) {
                counter++;
            }
        }
        return counter;
    }
    render() {
        return (
            <div>
                <div className={this.state.waitMode ? 'main-wait' : 'hide'}>
                </div>
                <div className="top-nav-menu row">
                    <div className="col-sm-8">
                        <button onClick={this.changeView('newbooks').bind(this)} className={this.state.view === 'newbooks' ? 'disabled' : ''}>
                            Новинки
                            </button>
                        <button onClick={this.changeView('wishlist').bind(this)} className={this.state.view === 'wishlist' ? 'disabled' : ''}>
                            Мой список
                            </button>
                        <button onClick={this.changeView('ignorelist').bind(this)} className={this.state.view === 'ignorelist' ? 'disabled' : ''}>
                            Скрываемые
                            </button>
                        <button onClick={this.changeView('favoritelist').bind(this)} className={this.state.view === 'favoritelist' ? 'disabled' : ''}>
                            Избранное
                            </button>
                        <button onClick={this.changeView('querylog').bind(this)} className={this.state.view === 'querylog' ? 'disabled' : ''}>
                            Журнал запросов
                            </button>
                        <button onClick={this.changeView('joblog').bind(this)} className={this.state.view === 'joblog' ? 'disabled' : ''}>
                            Журнал обновлений
                            </button>
                    </div>
                </div>
                <div className={this.state.view === 'newbooks' ? '' : 'hidden'}>
                    <div className="top-form row">
                        <div className="col-sm-3">
                            <h3>Новинки</h3>
                        </div>
                        <div className="col-sm-6">
                            <select value={this.state.categoryId} onChange={this.handleCategoryChange.bind(this)}>
                                {this.state.categories.map((e, key) => {
                                    return <option key={key} value={e.id}>{e.category_name}</option>;
                                })}
                            </select>
                            <DatePicker
                                dateFormat="P"
                                locale='ru'
                                selected={this.state.dateFrom}
                                onChange={this.handleDateFromChange.bind(this)}
                            />
                            -
                        <DatePicker
                                dateFormat="P"
                                locale='ru'
                                selected={this.state.dateTo}
                                onChange={this.handleDateToChange.bind(this)}
                            />
                            <button onClick={this.newBooksUpdate.bind(this)} className="search-button">Найти</button>
                        </div>
                        <div className="col-sm-3">
                            <span className="booksShown">Показано: {this.state.booksShown}</span>
                            <span className="booksIgnored">Скрыто: {this.state.booksIgnored}</span>
                            <span className="booksFavorite">Отмечено: {this.state.booksFavorite}</span>
                        </div>
                    </div>
                    <div className={this.state.booksShown > 0 ? 'hidden' : 'book-area empty-book-area'}>
                        Список пуст ({this.state.categoryName})
                    </div>
                    <div className="book-area">
                        {this.createBooks()}
                    </div>
                </div>
                <div className={this.state.view === 'ignorelist' ? '' : 'hidden'}>
                    <div className="top-form row">
                        <div className="col-sm-3">
                            <h3>Игнор-лист</h3>
                        </div>
                        <div className="col-sm-6">
                        </div>
                        <div className="col-sm-3">
                        </div>
                    </div>
                    <div className={this.getIgnoreListLength() > 0 ? 'hidden' : 'book-area empty-book-area ignore-area'}>
                        Список пуст
                    </div>
                    <div className="book-area ignore-area">
                        {this.createIgnoreList()}
                    </div>
                </div>
                <div className={this.state.view === 'favoritelist' ? '' : 'hidden'}>
                    <div className="top-form row">
                        <div className="col-sm-3">
                            <h3>Избранное</h3>
                        </div>
                        <div className="col-sm-6">
                        </div>
                        <div className="col-sm-3">
                        </div>
                    </div>
                    <div className={this.getFavoriteListLength() > 0 ? 'hidden' : 'book-area empty-book-area favorite-area'}>
                        Список пуст
                    </div>
                    <div className="book-area favorite-area">
                        {this.createFavoriteList()}
                    </div>
                </div>
                <div className={this.state.view === 'wishlist' ? '' : 'hidden'}>
                    <div className="top-form row">
                        <div className="col-sm-3">
                            <h3>Мой список</h3>
                        </div>
                        <div className="col-sm-6">
                            <input type="checkbox" checked={this.state.showWishlistAsText} onChange={this.handleWishListModeChange.bind(this)} />Как текст
                        </div>
                        <div className="col-sm-3">
                            <span className="booksShown">Всего: {this.getWishListLength()}</span>
                        </div>
                    </div>
                    <div className={this.getWishListLength() > 0 ? 'hidden' : 'book-area empty-book-area'}>
                        Список пуст
                    </div>
                    <div className={!this.state.showWishlistAsText ? "book-area" : "hidden"}>
                        {this.createWishList()}
                    </div>
                    <div className={this.state.showWishlistAsText ? "book-area" : "hidden"}>
                        {this.createWishListAsText()}
                    </div>
                </div>
                <div className={this.state.view === 'querylog' ? '' : 'hidden'}>
                    <div className="top-form row">
                        <div className="col-sm-3">
                            <h3>Журнал запросов</h3>
                        </div>
                        <div className="col-sm-6">
                        </div>
                        <div className="col-sm-3">
                        </div>
                    </div>
                    <div className={this.state.querylog.length > 0 ? 'hidden' : 'book-area empty-book-area'}>
                        Список пуст
                    </div>
                    <div className="book-area">
                        {this.createQueryLog()}
                    </div>
                </div>
                <div className={this.state.view === 'joblog' ? '' : 'hidden'}>
                    <div className="top-form row">
                        <div className="col-sm-3">
                            <h3>Журнал обновлений</h3>
                        </div>
                        <div className="col-sm-6">
                        </div>
                        <div className="col-sm-3">
                        </div>
                    </div>
                    <div className={this.state.joblog.length > 0 ? 'hidden' : 'book-area empty-book-area'}>
                        Список пуст
                    </div>
                    <div className="book-area">
                        {this.createJobLog()}
                    </div>
                </div>
            </div>
        );
    }
}
