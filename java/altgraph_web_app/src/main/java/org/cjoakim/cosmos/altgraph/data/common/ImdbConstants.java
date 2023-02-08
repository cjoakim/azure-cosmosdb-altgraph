package org.cjoakim.cosmos.altgraph.data.common;

/**
 * This interface defines constant values for IMDb People and Movies.
 * <p>
 * Chris Joakim, Microsoft, November 2022
 */
public interface ImdbConstants {

    public static final String MOVIE_FOOTLOOSE = "tt0087277";
    public static final String MOVIE_PRETTY_WOMAN = "tt0100405";

    public static final String MOVIE_RED_SPARROW = "tt2873282";

    public static final String MOVIE_BLADE_RUNNER = "tt0083658";

    public static final String[] MOVIES_OF_INTEREST = {
            MOVIE_FOOTLOOSE,
            MOVIE_PRETTY_WOMAN,
            MOVIE_RED_SPARROW,
            MOVIE_BLADE_RUNNER
    };

    public static final String PERSON_KEVIN_BACON = "nm0000102";
    public static final String PERSON_LORI_SINGER = "nm0001742";
    public static final String PERSON_JULIA_ROBERTS = "nm0000210";
    public static final String PERSON_JENNIFER_LAWRENCE = "nm2225369";
    public static final String PERSON_CHARLOTTE_RAMPLING = "nm0001648";

    public static final String[] PEOPLE_OF_INTEREST = {
            PERSON_KEVIN_BACON,
            PERSON_LORI_SINGER,
            PERSON_JULIA_ROBERTS,
            PERSON_JENNIFER_LAWRENCE,
            PERSON_CHARLOTTE_RAMPLING
    };

    // https://www.imdb.com/name/nm0000102/  Kevin Bacon
    public static final String URL_NAME_INFO_PREFIX = "https://www.imdb.com/name/";

    // https://www.imdb.com/title/tt0087277/  Footloose
    public static final String URL_TITLE_INFO_PREFIX = "https://www.imdb.com/title/";

//nm0000102 = kevin_bacon
//nm0000113 = sandra_bullock
//nm0000126 = kevin_costner
//nm0000148 = harrison_ford
//nm0000152 = richard_gere
//nm0000158 = tom_hanks
//nm0000163 = dustin_hoffman
//nm0000178 = diane_lane
//nm0000206 = keanu_reeves
//nm0000210 = julia_roberts
//nm0000234 = charlize_theron
//nm0000456 = holly_hunter
//nm0000518 = john_malkovich
//nm0000849 = javier_bardem
//nm0001648 = charlotte_rampling
//nm0001742 = lori_singer
//nm0001848 = dianne_wiest
//nm0005476 = hilary_swank
//nm0177896 = bradley_cooper
//nm0205626 = viola_davis
//nm1297015 = emma_stone
//nm2225369 = jennifer_lawrence

}
