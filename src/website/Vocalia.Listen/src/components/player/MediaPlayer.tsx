import React, { Component, PureComponent } from "react";
import { IconButton, Fab, Card } from "@material-ui/core";
import {
  Forward30,
  Replay10,
  PlayArrow,
  Pause,
  VolumeUp
} from "@material-ui/icons";
import Slider from "@material-ui/lab/Slider";
import "./MediaPlayer.css";
import { PodcastEpisode } from "../../types";
import { formatTime } from "../../utility/FormatUtils";

interface IProps {
  media: PodcastEpisode;
  isMobile: boolean;
}

interface IState {
  paused: boolean;
  time: number;
  volume: number;
  audioObject: HTMLAudioElement;
}

export default class MediaPlayer extends PureComponent<IProps, IState> {
  constructor(props: IProps) {
    super(props);

    const { media } = this.props;
    let audioObject = document.createElement("audio");
    audioObject.loop = false;
    audioObject.currentTime = media.time;
    audioObject.ontimeupdate = () => this.onHandleTimeUpdate();
    audioObject.onended = () => this.setState({ paused: true });

    this.state = {
      paused: true,
      time: media.time,
      volume: 0.7,
      audioObject: audioObject
    };
  }

  componentWillReceiveProps(props: IProps) {
    const { audioObject } = this.state;

    if (props.media.content !== audioObject.src) {
      this.initializePodcastFromProps(props);
    }
  }

  componentWillMount() {
    const { isMobile } = this.props;
    this.initializePodcastFromProps(this.props);

    this.setState({
      volume: isMobile ? 1 : this.state.volume
    });
  }

  /**
   * Loads a podacst from the props source into the player.
   */
  initializePodcastFromProps = (props: IProps) => {
    const { audioObject } = this.state;
    audioObject.src = props.media.content;
    audioObject.load();
    audioObject.play();
    this.setState({ paused: false });
  };

  /**
   * Called when a fast-forward event has occured.
   */
  onForward = () => {
    const { audioObject } = this.state;

    const currentTime = audioObject.currentTime + 30;
    audioObject.currentTime = currentTime;
    this.setState({ time: currentTime });
  };

  /**
   * Called when a rewind event has occured.
   */
  onRewind = () => {
    const { audioObject } = this.state;

    const currentTime = audioObject.currentTime - 10;
    audioObject.currentTime = currentTime;
    this.setState({ time: currentTime });
  };

  /**
   * Called when the play/pause button has been toggled.
   */
  onTogglePause = () => {
    const { audioObject } = this.state;

    if (this.state.paused) {
      audioObject.play();
      this.setState({ paused: false });
    } else {
      audioObject.pause();
      this.setState({ paused: true });
    }
  };

  /**
   * Called when a seek has occured.
   */
  onSeek = (e: any, v: number) => {
    const { audioObject } = this.state;

    if (v <= audioObject.duration) {
      this.setState({ time: v });
      audioObject.currentTime = v;
    }
  };

  /**
   * Called when the volume has changed.
   */
  onChangeVolume = (e: any, v: number) => {
    const { audioObject } = this.state;

    this.setState({ volume: v });
    audioObject.volume = v;
  };

  /**
   * Updates the current time in the component state.
   */
  onHandleTimeUpdate = () => {
    const { audioObject } = this.state;
    this.setState({ time: audioObject.currentTime });
  };

  render() {
    let icon;

    if (this.state.paused) {
      icon = <PlayArrow />;
    } else {
      icon = <Pause />;
    }

    const { media, isMobile } = this.props;
    const { time, volume, audioObject } = this.state;

    return (
      <Card
        className="player"
        style={{
          width: "100%",
          position: "sticky",
          bottom: 0
        }}
      >
        <div className="player-left">
          {!isMobile && media.imageUrl != null && (
            <div className="image">
              {media.imageUrl != null}
              <img alt="podcast-logo" src={media.imageUrl} />
            </div>
          )}

          <IconButton className="icon" onClick={this.onRewind}>
            <Replay10 />
          </IconButton>

          <Fab size="small" color="primary" onClick={this.onTogglePause}>
            {icon}
          </Fab>

          <IconButton className="icon" onClick={this.onForward}>
            <Forward30 />
          </IconButton>
        </div>

        <div className="player-center">
          <div className="no-wrap">
            <div className="episode">
              <span className="episode-title">{media.title}</span>
            </div>

            <div className="title">
              <span className="podcast-title">{media.author}</span>
            </div>
          </div>

          <div className="seek-bar">
            <Slider
              min={0}
              max={audioObject.duration}
              value={time}
              onChange={this.onSeek}
            />
            <span className="time-text current-time">{formatTime(time)}</span>

            <span className="time-text time-remaining">
              {formatTime(
                isNaN(audioObject.duration) ? 0 : audioObject.duration
              )}
            </span>
          </div>
        </div>

        {!isMobile && (
          <div className="player-right">
            <VolumeUp className="icon" />
            <Slider
              min={0}
              max={1}
              value={volume}
              onChange={this.onChangeVolume}
            />
          </div>
        )}
      </Card>
    );
  }
}
