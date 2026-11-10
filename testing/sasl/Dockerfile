FROM debian:bookworm-slim
RUN apt-get update && apt-get install -y --no-install-recommends \
    subversion sasl2-bin libsasl2-modules && rm -rf /var/lib/apt/lists/*
COPY start.sh /start.sh
RUN sed -i 's/\r$//' /start.sh
EXPOSE 3690
CMD ["sh", "/start.sh"]
