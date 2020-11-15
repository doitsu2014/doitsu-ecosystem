/// <binding Clean='clean' />

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";

paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";

paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

function cleanAll(cb) {
    rimraf(paths.concatJsDest, cb);
    rimraf(paths.concatCssDest, cb);
}
cleanAll.displayName = "clean:all";
gulp.task(cleanAll);

function minAll(cb) {
    function minJs() {
        (gulp.src([paths.js, "!" + paths.minJs], { base: "." })
            .pipe(concat(paths.concatJsDest))
            .pipe(uglify())
            .pipe(gulp.dest(".")));
    }

    function minCss() {
        (gulp.src([paths.css, "!" + paths.minCss])
            .pipe(concat(paths.concatCssDest))
            .pipe(cssmin())
            .pipe(gulp.dest(".")));
    }

    gulp.series(minJs, minCss);
    cb();
}
minAll.displayName = "min:all";
gulp.task(minAll);