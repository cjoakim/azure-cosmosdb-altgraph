#!/bin/bash

# Bash and curl script to download the IMDb datasets on linux/WSL/macOS.
# See https://datasets.imdbws.com
# Chris Joakim, Microsoft, November 2022

echo 'removing output files ...'
mkdir -p imdb_raw/

rm    imdb_raw/*.gz
rm    imdb_raw/*.tsv

echo "getting name.basics ..."
curl "https://datasets.imdbws.com/name.basics.tsv.gz" > imdb_raw/name.basics.tsv.gz

echo "getting title.akas ..."
curl "https://datasets.imdbws.com/title.akas.tsv.gz" > imdb_raw/title.akas.tsv.gz

echo "getting title.basics ..."
curl "https://datasets.imdbws.com/title.basics.tsv.gz" > imdb_raw/title.basics.tsv.gz

echo "getting title.crew ..."
curl "https://datasets.imdbws.com/title.crew.tsv.gz" > imdb_raw/title.crew.tsv.gz

echo "getting title.episode ..."
curl "https://datasets.imdbws.com/title.episode.tsv.gz" > imdb_raw/title.episode.tsv.gz

echo "getting title.principals ..."
curl "https://datasets.imdbws.com/title.principals.tsv.gz" > imdb_raw/title.principals.tsv.gz

echo "getting title.ratings ..."
curl "https://datasets.imdbws.com/title.ratings.tsv.gz" > imdb_raw/title.ratings.tsv.gz

ls -al imdb_raw/

echo 'unzipping'
cd    imdb_raw

gunzip name.basics.tsv.gz
gunzip title.akas.tsv.gz
gunzip title.basics.tsv.gz
gunzip title.crew.tsv.gz
gunzip title.episode.tsv.gz
gunzip title.principals.tsv.gz
gunzip title.ratings.tsv.gz

ls -al

cd ..
echo 'done'
