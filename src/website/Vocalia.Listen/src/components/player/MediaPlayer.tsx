import React, { PureComponent } from "react";
import { IconButton, Fab, Card, Typography, Fade } from "@material-ui/core";
import {
  Forward30,
  Replay10,
  PlayArrow,
  Pause,
  VolumeUp
} from "@material-ui/icons";
import Slider from "@material-ui/lab/Slider";
import "./MediaPlayer.css";
import { PodcastEpisode } from "../../utility/types";
import { formatTime } from "../../utility/FormatUtils";
import { Link } from "react-router-dom";

/**
 * Required properties for the player.
 */
interface IPlayerProps {
  media: PodcastEpisode; //The media to be played.
  isMobile: boolean; //Indicates if the current device is a mobile device.
}

/**
 * State information belonging to the player.
 */
interface IPlayerState {
  paused: boolean; //Indicates if playback has been paused.
  time: number; //The current progress of playback.
  volume: number; //Volume of playback.
  audioObject: HTMLAudioElement; //HTML Audio element for audio control.
  imageLoaded: boolean; //Indicates if the image has loaded in.
}

/**
 * Material-themed player for podcast episode objects.
 */
export default class MediaPlayer extends PureComponent<
  IPlayerProps,
  IPlayerState
> {
  /**
   * Initializes the audio object for playback.
   * @param props Required propeties.
   */
  constructor(props: IPlayerProps) {
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
      volume: 0.5,
      audioObject: audioObject,
      imageLoaded: false
    };
  }

  /**
   * Called when a new episode has been selected.
   * @param props Passed properties.
   */
  componentWillReceiveProps(props: IPlayerProps) {
    const { audioObject } = this.state;

    if (props.media.content !== audioObject.src) {
      this.initializePodcastFromProps(props);
    }
  }

  /**
   * Called before initial mounting, sets initial mobile volume state, and sets the media source.
   */
  componentWillMount() {
    const { isMobile } = this.props;
    this.setState({ imageLoaded: false });
    this.initializePodcastFromProps(this.props);

    this.setState({
      volume: isMobile ? 1 : this.state.volume
    });
  }

  /**
   * Called before unmounting, and stops the audio source from playing.
   */
  componentWillUnmount() {
    this.state.audioObject.pause();
  }

  /**
   * Loads a podacst from the props source into the player.
   */
  initializePodcastFromProps = (props: IPlayerProps) => {
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
    let icon = this.state.paused ? <PlayArrow /> : <Pause />;

    const { media, isMobile } = this.props;
    const { time, volume, audioObject, imageLoaded } = this.state;

    return (
      <Card
        className="player"
        style={{
          width: "100%",
          position: "sticky",
          bottom: 0
        }}
      >
        {/* Image & Playback */}
        <div className="player-left">
          {!isMobile && media.imageUrl != null && (
            <div className="image">
              {media.imageUrl != null}
              <Fade in={imageLoaded}>
                <img
                  alt="podcast-logo"
                  src={media.imageUrl}
                  onLoad={() => this.setState({ imageLoaded: true })}
                />
              </Fade>
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

        {/* Seek & Naming */}
        <div className="player-center">
          <div className="no-wrap">
            <div className="episode">
              <span className="episode-title">
                <Typography component="h6" variant="h6">
                  {media.title}
                </Typography>
              </span>
            </div>

            <div className="title">
              <span className="podcast-title">
                <Link
                  to={"/detail/" + encodeURIComponent(media.rssUrl)}
                  style={{ textDecoration: "none" }}
                >
                  <Typography component="p" color="textSecondary">
                    {media.author}
                  </Typography>
                </Link>
              </span>
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

        {/* Volume Control */}
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
