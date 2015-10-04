# F# TV Web Course

This repository is your foundation for building your Suave web site.

## Structure

The root contains everything you need to compile and manage the project. It
could be built with FAKE, too, but I'm going to leave that as an exercise to the
reader, until the code has stabilised, at which point I shall overwrite this
statement and provide a FAKE implementation. After all, having Node, Ruby and F#
in the same repository is almost stranger than what github's language statistics
engine really can deal with, even on a sunny day!

The `src/` folder contains the `Web/` and the JavaScript in `public/`. Pretty
much all modern sites nowadays are built against an API with JavaScript, rather
than having the server generate all the pages. As a F# is a modern language with
a modern web framework, Suave, we like to stay with what works well for
everybody, and provide the same facilities.

In `src/` you can find a `.sln` file that opens a simple console project with a
F# application written with Suave.

In `src/public/` you'll find a `package.json` file that specifies the NPM
dependencies that you build the browser-facing parts of your web site with. You
should run `npm install` to make 'it come true', what is in the `package.json`
file.

The `Procfile` contains the information needed to start both a development NPM
server that runs continuously in the background, compiling, minimising and
compressing your JS to the `./build/public` folder, as well as a command that
simply runs the console application that is a Suave web server.

While Procfiles used to be Heroku-specific, there's a tool, foreman, that lets
you use them on both Windows, OS X and Linux which makes it a lot easier to
write micro-services, or even adhere to the 12factor-app principles, because you
don't have to manage all the processes manually.

## Building

Either: `./build.sh` or `build.bat` or `foreman start` or `xbuild src/Web.sln`
or `msbuild src/Web.sln` depending on how you feel today =). You basically
cannot type anything into the console without building this application!


## Contributing

We will provide a way for contributing back fixes and strive to keep the source
code as updated with library changes as possible; so if you find something you'd
like to improve that doesn't change the semantics of the course, do send us a
pull request!

You should send a PR against `master`.

## TODO:

 - [ ] Use Node v4
 - [ ] Use FAKE
 - [ ] Implement the sample app itself, not just the scaffolding
 - [ ] Set up branches appropriately (tags?)
 - [ ] Ensure we can push NPM + .Net to Heroku [as a dual buildpack](https://github.com/ademar/suave-presentation.2015-09-03/commits/master) ([and here](https://devcenter.heroku.com/changelog-items/653)) and it all just works
