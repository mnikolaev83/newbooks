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
        return prevMonday;
    }

    static displayName = Home.name;
    constructor() {
        super();
        var dateFrom = this.getPreviousMonday();
        this.state = { categoryId: 0, categories: [], dateFrom: dateFrom, dateTo: new Date(), books: [], booksShown: 0, booksIgnored: 0, booksFavorite: 0 };
        fetch('http://newbooksapi/api/categories')
            .then(response => response.json())
            .then(data => {
                this.setState({ categories: data, categoryId: data[0].id });
            });
    }
    handleCategoryChange(event) {
        this.setState({ categoryId: event.target.value });
    }
    handleDateFromChange(date) {
        this.setState({ dateFrom: date });
    }
    handleDateToChange(date) {
        this.setState({ dateTo: date });
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
                this.setState({ books: data, booksIgnored: booksIgnoredCounter, booksShown: booksShownCounter, booksFavorite: booksFavoriteCounter });

            });
    }
    createBooks() {
        let books = [];
        var counter = 0;
        for (let i = 0; i < this.state.books.length; i++) {
            if (!this.state.books[i].is_ignored) {
                books.push(
                    <Book data={this.state.books[i]} number={counter + 1} />
                )
                counter++;
            }
        }
        return books;
    }
    render() {
        return (
            <div>
                <div className="ignore-list-popup hidden">
                    <div className="popup-fade">
                    </div>
                </div>
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
                <div className="book-area">
                    {this.createBooks()}
                </div>
            </div>
        );
    }
}
