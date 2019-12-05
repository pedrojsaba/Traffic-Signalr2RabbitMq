CREATE TABLE outbox(
  outbox_id BIGINT NOT NULL AUTO_INCREMENT,
  message_id VARCHAR(255) NOT NULL,
  dispatched TINYINT(1) NOT NULL,
  dispatched_at DATETIME DEFAULT NULL,
  transport_operations VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (outbox_id),
  UNIQUE INDEX UQ_outbox_message_id(message_id),
  INDEX IX_outbox_dispatched(dispatched),
  INDEX IX_outbox_dispatched_at(dispatched_at)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

CREATE TABLE traffic(
  traffic_id BIGINT NOT NULL,
  register VARCHAR(36) NOT NULL,
  speed DECIMAL(10,2) NOT NULL,
  plate VARCHAR(36) NOT NULL,
  photo VARCHAR(50) NOT NULL,
  source_id BIGINT(20) NOT NULL,
  opened_at_utc DATETIME NOT NULL,
  updated_at_utc DATETIME NOT NULL,
  PRIMARY KEY(traffic_id),
  UNIQUE INDEX UQ_traffic_id(traffic_id)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;