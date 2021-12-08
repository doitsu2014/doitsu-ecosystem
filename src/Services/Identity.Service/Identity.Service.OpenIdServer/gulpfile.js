const {series, parallel, src, dest } = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    rename = require('gulp-rename'),
    babel = require('gulp-babel');

const paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

function cleanJs(cb) {
    rimraf(paths.concatJsDest, cb);
}

function cleanCss(cb) {
    rimraf(paths.concatCssDest, cb);
}

const clean = parallel(cleanCss, cleanJs);


function minJs(cb) {
    return src([paths.js, "!" + paths.minJs], {base: "."})
        .pipe(concat(paths.concatJsDest))
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(uglify())
        .pipe(dest("."))
}

function minCss(cb) {
    return src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(dest("."))
} 

const min = parallel(minJs, minCss);

// gulp.task('min:css', function () {
// });
//
// gulp.task("min", ["min:js", "min:css"]);

// gulp.task("default", ["clean", "min"])

exports.clean = clean;
exports.default = series(clean, min);