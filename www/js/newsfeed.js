$(function() {
    var newsDiv = $('#news');
    if(newsDiv.length) {
        $.getFeed({
            url: '/servlets/WebFeed?artifact=news',
            success: function(feed) {
                $.each(feed.items.slice(0, 3), function(index, item) {
                    var d = new Date(Date.parse(item.updated));
                    if(d.getFullYear() < 2009)
                        return;
                    var updated = d.toLocaleDateString();
                    var div = $('<div/>').addClass('h3');
                    
                    var title = $('<h3/>')
                        .text(item.title);
                    title.append($('<span style="margin-left: 15px; font-weight:normal"/>').text('(' + updated + ')'));
                    div.append(title);
                    
                    var description = $('<p/>').html(item.description);
                    div.append(description);
                    
                    newsDiv.append(div);
                });
            }
        });
    }
});