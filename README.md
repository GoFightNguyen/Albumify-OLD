![.NET Core](https://github.com/GoFightNguyen/Albumify/workflows/.NET%20Core/badge.svg)

# Albumify
I love music, and my favorite way to listen is by album.

## Local Setup
Create secrets for:
- SpotifyClientId
- SpotifyClientSecret

You can create/access your values at [Spotify](https://developer.spotify.com/dashboard/).

### Run MongoDB locally
`cd` into the docker directory.
This directory contains the Dockerfile and scripts to run at startup.

Create an image named `albumify-mongo`: `docker build . -t albumify-mongo`

Run a container named `albumify-mongo`: `docker run --name albumify-mongo -p 27017:27017 -d --rm albumify-mongo`

`exec` into the container: `docker exec -it albumify-mongo mongo -u 'albumify-user' -p 'albumify-password' --authenticationDatabase 'albumify'`