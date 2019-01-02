import React, { Component } from 'react';
import './Home.css';
import DatePicker from "react-datepicker";
import Book from "./Book";
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

    static displayName = Home.name;
    constructor() {
        super();
        var dateFrom = this.getPreviousMonday();
        this.state = { categoryId: 0, categories: [], dateFrom: dateFrom, dateTo: new Date(), books: [], booksShown: 0, booksIgnored: 0, booksFavorite: 0 };
        fetch('http://newbooksapi/api/categories')
            .then(response => response.json())
            .then(data => {
                var newState = this.state;
                newState.categories = data;
                newState.categoryId = data[0].id;
                this.setState(newState);
            });
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
    dataUpdate(event) {
        var encodedDateFrom = encodeURIComponent(dateFormat(this.state.dateFrom, "yyyy-mm-dd"));
        var encodedDateTo = encodeURIComponent(dateFormat(this.state.dateTo, "yyyy-mm-dd"));
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
                for (var i = 0; i < this.state.categories.length; i++) {
                    if (parseInt(this.state.categories[i].id) === parseInt(this.state.categoryId)) {
                        newState.categoryName = this.state.categories[i].category_name;
                    }
                }
                this.setState(newState);
            });
    }
    createBooks() {
        let books = [];
        var counter = 0;
        for (let i = 0; i < this.state.books.length; i++) {
            if (!this.state.books[i].is_ignored) {
                books.push(
                    <Book key={this.state.books[i].id} data={this.state.books[i]} number={counter + 1} />
                )
                counter++;
            }
        }
        return books;
    }
    render() {
        return (
            <div>
                <div className="top-form row">
                    <div className="col-sm-4">
                        Категория:
                        <select value={this.state.categoryId} onChange={this.handleCategoryChange.bind(this)}>
                            {this.state.categories.map((e, key) => {
                                return <option key={key} value={e.id}>{e.category_name}</option>;
                            })}
                        </select>
                    </div>
                    <div className="col-sm-4">
                        Добавлено:
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
                        <button onClick={this.dataUpdate.bind(this)} className="search-button">Найти</button>
                    </div>
                    <div className="col-sm-4">
                        <span className="booksShown">Показано: {this.state.booksShown}</span>
                        <span className="booksIgnored">Скрыто: {this.state.booksIgnored}</span>
                        <span className="booksFavorite">Отмечен: {this.state.booksFavorite}</span>
                    </div>
                </div>
                <div className={this.state.booksShown > 0 ? 'hidden' : 'book-area empty-book-area'}>
                    Список пуст ({this.state.categoryName})
                    </div>
                <div className="book-area">
                    {this.createBooks()}
                </div>
            </div>
        );
    }
}
