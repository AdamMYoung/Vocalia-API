import React, { Component } from "react";
import "./App.css";

class App extends Component {
  render() {
    return (
      <div className="App">
        <header className="App-header">
          <a>{process.env.REACT_APP_API_URL}</a>
        </header>
      </div>
    );
  }
}

export default App;
