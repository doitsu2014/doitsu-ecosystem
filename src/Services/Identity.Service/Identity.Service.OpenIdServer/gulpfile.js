const gulp = require("gulp"),
    del = require("del"),
    vinylPaths = require('vinyl-paths'),
    rename = require('gulp-rename'),
    uglify = require('gulp-uglify'),
    babel = require('gulp-babel'),
    cleanCss = require('gulp-clean-css');

const paths = {
    webroot: "wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.distJs = paths.webroot + "dist/js/";
paths.distCss = paths.webroot + "dist/css/";

gulp.task('clean:script', function () {
    return gulp.src(paths.distJs, {allowEmpty: true, read: false})
        .pipe(vinylPaths(del))
        .pipe(gulp.dest(paths.distJs));
});

gulp.task('clean:css', function () {
    return gulp.src(paths.distCss, {allowEmpty: true, read: false})
        .pipe(vinylPaths(del))
        .pipe(gulp.dest(paths.distCss));
});

gulp.task('min:script', function () {
    return gulp.src([paths.js, "!" + paths.minJs])
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(uglify())
        .pipe(rename({ extname: '.min.js' }))
        .pipe(gulp.dest(paths.distJs))
});

gulp.task('min:css', function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(cleanCss({compatibility: 'ie8'}))
        .pipe(rename({ extname: '.min.css' }))
        .pipe(gulp.dest(paths.distCss))
});

gulp.task('default', gulp.series(['clean:script', 'clean:css', 'min:script', 'min:css']));