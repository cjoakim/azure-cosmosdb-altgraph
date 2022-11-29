# azure-cosmosdb-altgraph - raw datasets

## NPM Data Files

The raw data has already been downloaded and curated into this repo;
so you do not need to download any NPM files to use this AltGraph repo.

See the **altgraph_data_app/data/npm_refined/** directory.

## IMDb Data Files

This repo also has a "refined", or curated, set of IMDb data.
Please unzip these files; they are stored as zip files due to GitHub
filesize limitations:

See the **altgraph_data_app/data/imdb_refined/** directory.

```
altgraph_data_app/data/imdb_refined/movies.zip
altgraph_data_app/data/imdb_refined/movies_idx.zip
altgraph_data_app/data/imdb_refined/people.zip
```

Optionally, if you want a smaller or larger dataset, you can download
the (large) raw IMDb TSV files and "wrangle" them with the code in this repo.
These three files are required.

```
IMDB_RAW_NAME_BASICS_FILE      = "data/imdb_raw/name.basics.tsv";
IMDB_RAW_TITLE_BASICS_FILE     = "data/imdb_raw/title.basics.tsv";
IMDB_RAW_TITLE_PRINCIPALS_FILE = "data/imdb_raw/title.principals.tsv";
```

### IMDb Links

You can download these raw TSV files here.

- https://datasets.imdbws.com/
- https://www.imdb.com/interfaces/
