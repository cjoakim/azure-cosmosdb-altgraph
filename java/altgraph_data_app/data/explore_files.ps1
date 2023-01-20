
# Explore the size and shape of the IMDb CSV files.
# Chris Joakim, Microsoft, November 2022

echo ""
echo "name.basics.tsv"
cat .\imdb\raw\name.basics.tsv | Select-Object -first 3
cat .\imdb\raw\name.basics.tsv | Measure-Object -Line

echo ""
echo "title.basics.tsv"
cat .\imdb\raw\title.basics.tsv | Select-Object -first 3
cat .\imdb\raw\title.basics.tsv | Measure-Object -Line

echo ""
echo "title.principals.tsv"
cat .\imdb\raw\title.principals.tsv | Select-Object -first 3
cat .\imdb\raw\title.principals.tsv | Measure-Object -Line

echo ""

# name.basics.tsv
# nconst  primaryName     birthYear       deathYear       primaryProfession       knownForTitles
# nm0000001       Fred Astaire    1899    1987    soundtrack,actor,miscellaneous  tt0031983,tt0050419,tt0053137,tt0072308
# nm0000002       Lauren Bacall   1924    2014    actress,soundtrack      tt0037382,tt0038355,tt0071877,tt0117057
# 11978492

# title.basics.tsv
# tconst  titleType       primaryTitle    originalTitle   isAdult startYear       endYear runtimeMinutes  genres
# tt0000001       short   Carmencita      Carmencita      0       1894    \N      1       \N
# tt0000002       short   Le clown et ses chiens  Le clown et ses chiens  0       1892    \N      5       \N
#  9275715

# title.principals.tsv
# tconst  ordering        nconst  category        job     characters
# tt0000001       1       nm1588970       self    \N      ["Self"]
# tt0000001       2       nm0005690       director        \N      \N
# 52412048

# -a----         10/5/2022  11:10 AM      717118399 name.basics.tsv
# -a----         10/5/2022  11:18 AM     1618554293 title.akas.tsv
# -a----         10/5/2022  11:19 AM      711430966 title.basics.tsv
# -a----         10/5/2022  11:19 AM      303680263 title.crew.tsv
# -a----         10/5/2022  11:20 AM      181682399 title.episode.tsv
# -a----         10/5/2022  11:21 AM     2308685550 title.principals.tsv
# -a----         10/5/2022  11:21 AM       21819279 title.ratings.tsv
