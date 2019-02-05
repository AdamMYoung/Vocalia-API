import React, { Component } from "react";
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
import { isMobile } from "../../utility/DeviceUtils";
import { formatTime } from "../../utility/FormatUtils";

interface IProps {
  media: PodcastEpisode;
}

interface IState {
  audioObject: HTMLAudioElement;
  paused: boolean;
  time: number;
  volume: number;
}

export default class MediaPlayer extends Component<IProps, IState> {
  constructor(props: any) {
    super(props);

    const { media } = this.props;

    let audio = document.createElement("audio");
    audio.loop = false;
    audio.currentTime = media.time;
    audio.ontimeupdate = () => this.onHandleTimeUpdate();
    audio.onended = () => this.setState({ paused: true });

    this.state = {
      paused: true,
      audioObject: audio,
      time: media.time,
      volume: 0.7
    };
  }

  componentWillMount() {
    const { audioObject } = this.state;

    console.log(this.props.media.content);
    audioObject.src = this.props.media.content;
    audioObject.play();
    this.setState({ paused: false });
  }

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
   * Called when the play/pause button has been toggle
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

    const { media } = this.props;
    const { audioObject, time, volume } = this.state;

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
          {!isMobile() && media.imageUrl != null && (
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

        {!isMobile() && (
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
