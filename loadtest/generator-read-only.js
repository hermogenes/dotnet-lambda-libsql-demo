module.exports = {
  getRandomId: function (requestParams, response, context, events, done) {
    if (response.statusCode !== 200) {
      return done(new Error("Failed to get items"));
    }
    var items = JSON.parse(response.body);
    context.vars.id = items[Math.floor(Math.random() * items.length)].id;

    return done();
  },
};
