# Vocal Replace Medusa
A Medusa (plugin/extension) for the discord bot Nadeko. Connects with a separate project that provides an API for TTS and youtube video voice replacement. [streamtts](https://github.com/alexankitty/streamtts) also needs to be setup alongside this project.

# Commands
```
.voicereplaceryt: url, voice, pitch
    Replaces the vocals of a youtube video via an RVC AI
.voices:
    Grabs a list of available voices from the endpoint
.voicereplacerapi:
    (Bot Owner) Sets the location of the API server. Format: http://127.0.0.1:8081
```