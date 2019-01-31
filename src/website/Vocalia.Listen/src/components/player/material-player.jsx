import React, { Component } from "react";
import { IconButton, Fab, Card, Slide } from "@material-ui/core";
import {
  Forward30,
  Replay10,
  PlayArrow,
  Pause,
  VolumeUp
} from "@material-ui/icons";
import Slider from "@material-ui/lab/Slider";
import "./material-player.css";

/**
 * Material-design audio player. Handles audio streams and provides play/pause, scrubbing and volume control.
 * Prop List:
 * src: Path to the audio content to play.
 * image: Path to the content thumbnail.
 * podcast: Name of the podcast.
 * episode: Name of the episode.
 * volume: (Optional) Volume the player should be set at. Range between 0.0 - 1.0.
 * autoplay: (Optional) True/False if the player should start instantly.
 * time: (Optional) Time the player should start at in seconds.
 */
export default class MaterialPlayer extends Component {
  constructor(props) {
    super(props);

    const time = this.props.time == null ? 0 : this.props.time;
    const volume = this.props.volume == null ? 0.5 : this.props.volume;
    const autoplay = this.props.autoplay == null ? false : this.props.autoplay;

    this.state = {
      paused: true,
      volume: volume,
      displayedTime: time
    };

    //Setup UI Hooks for buttons & sliders.
    this.forward = () => {
      const currentTime = this.audio.currentTime + 30;
      this.audio.currentTime = currentTime;
      this.setState({ displayedTime: currentTime });
    };

    this.rewind = () => {
      const currentTime = this.audio.currentTime - 10;
      this.audio.currentTime = currentTime;
      this.setState({ displayedTime: currentTime });
    };

    this.togglePause = () => {
      if (this.state.paused) {
        this.audio.play();
        this.setState({ paused: false });
      } else {
        this.audio.pause();
        this.setState({ paused: true });
      }
    };

    this.seek = (e, v) => {
      if (v <= this.audio.duration) {
        this.setState({ displayedTime: v });
        this.audio.currentTime = v;
      }
    };

    this.changeVolume = (e, v) => {
      this.setState({ volume: v });
    };

    this.updatePredicate = this.updatePredicate.bind(this);

    //Initialize HTML5 audio object and assign event handlers.
    this.audio = document.createElement("audio");
    this.audio.src = this.props.src;
    this.audio.loop = false;
    this.audio.currentTime = this.state.displayedTime;
    this.audio.ontimeupdate = () => this.handleTimeUpdate();
    this.audio.onended = () => this.setState({ paused: true });

    if (autoplay && this.state.paused) {
      this.audio.play();
      this.state.paused = false;
    }
  }

  /**
   * Called when the component is loaded, and assigns event listeners to the resize event.
   */
  componentDidMount() {
    this.updatePredicate();
    window.addEventListener("resize", this.updatePredicate);

    if (!this.state.isDesktop) {
      this.setState({ volume: 1 });
    }
  }

  /**
   * Called when the component is unloaded.
   */
  componentWillUnmount() {
    window.removeEventListener("resize", this.updatePredicate);
  }

  /**
   * Updates the current time in the component state.
   */
  handleTimeUpdate() {
    const time = this.audio.currentTime;
    this.setState({ displayedTime: time });
  }

  /**
   * Called when the resize event is called.
   */
  updatePredicate() {
    this.setState({ isDesktop: window.innerWidth > 700 });
  }

  render() {
    let icon;

    if (this.state.paused) {
      icon = <PlayArrow />;
    } else {
      icon = <Pause />;
    }

    const isDesktop = this.state.isDesktop;
    const podcastSelected = this.props.src != null;

    return (
      <Slide direction="up" in={podcastSelected}>
        <Card className="player" raised={true}>
          <div className="player-left">
            {isDesktop && this.props.image != null && (
              <div className="image">
                {this.props.image != null}
                <img alt="podcast-logo" src={this.props.image} />
              </div>
            )}

            <IconButton className="icon" onClick={this.rewind}>
              <Replay10 />
            </IconButton>

            <Fab size="small" color="primary" onClick={this.togglePause}>
              {icon}
            </Fab>

            <IconButton className="icon" onClick={this.forward}>
              <Forward30 />
            </IconButton>
          </div>

          <div className="player-center">
            <div className="no-wrap">
              <div className="episode">
                <span className="episode-title">{this.props.episode}</span>
              </div>

              <div className="title">
                <span className="podcast-title">{this.props.podcast}</span>
              </div>
            </div>

            <div className="seek-bar">
              <Slider
                min={0}
                max={this.audio.duration}
                value={this.state.displayedTime}
                onChange={this.seek}
              />
              <span className="time-text current-time">
                {formatTime(this.state.displayedTime)}
              </span>

              <span className="time-text time-remaining">
                {formatTime(
                  isNaN(this.audio.duration) ? 0 : this.audio.duration
                )}
              </span>
            </div>
          </div>

          {isDesktop && (
            <div className="player-right">
              <VolumeUp className="icon" />
              <Slider
                min={0}
                max={1}
                value={this.audio.volume}
                onChange={this.changeVolume}
                onDragLeave={(this.audio.volume = this.state.volume)}
              />
            </div>
          )}
        </Card>
      </Slide>
    );
  }
}

/**
 * Formats seconds into HH:MM:SS, or MM:SS if under an hour.
 * @param {Number (in seconds) to format.} num
 */
function formatTime(num) {
  var sec_num = parseInt(num, 10); // don't forget the second param
  var hours = Math.floor(sec_num / 3600);
  var minutes = Math.floor((sec_num - hours * 3600) / 60);
  var seconds = sec_num - hours * 3600 - minutes * 60;

  if (hours < 10) {
    hours = "0" + hours;
  }
  if (minutes < 10) {
    minutes = "0" + minutes;
  }
  if (seconds < 10) {
    seconds = "0" + seconds;
  }

  if (hours === "00") {
    return minutes + ":" + seconds;
  } else {
    return hours + ":" + minutes + ":" + seconds;
  }
}
